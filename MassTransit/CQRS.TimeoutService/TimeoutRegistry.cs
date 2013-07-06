using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Common;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit.Services.Timeout.Messages;

namespace MHM.WinFlexOne.CQRS.TimeoutService
{
    public class TimeoutRegistry
    {
        private readonly object _lockObject = new object();
        private readonly Dictionary<string, DateTime> _idExpiresMap = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, Timer> _idTimerMap = new Dictionary<string, Timer>();
        private readonly IEventStore _eventStore;
        private readonly Action<TimeoutElapsedEvent> _timeoutCallBack;
        private bool _initializing;
        private bool _initialized;

        public TimeoutRegistry(IEventStore eventStore, Action<TimeoutElapsedEvent> timeoutCallBack)
        {
            _eventStore = eventStore;
            _timeoutCallBack = timeoutCallBack;
        }

        public void RegisterTimeout(StartTimeout command)
        {
            //critical region that can have potentially many threads running through it
            lock (_lockObject)
            {
                if (_idExpiresMap.ContainsKey(command.CorrelationId) == false)
                {
                    _idExpiresMap.Add(command.CorrelationId, CalculateExpiration(command.ElapsesInMS));
                }
                else
                {
                    _idExpiresMap[command.CorrelationId] = CalculateExpiration(command.ElapsesInMS);
                }

                if (_idTimerMap.ContainsKey(command.CorrelationId) == false)
                {
                    _idTimerMap.Add(command.CorrelationId, NewTimer(command.ElapsesInMS));
                }
                else
                {
                    _idTimerMap[command.CorrelationId] = NewTimer(command.ElapsesInMS);
                }
            }
        }

        private void Initialize()
        {
            _initializing = true;
            try
            {
                //read all events from the event store and rebuild the maps

            }
            finally
            {
                _initializing = false;
            }

        }

        private Timer NewTimer(int expiresMS)
        {
            var timer = new Timer(expiresMS);
            timer.Elapsed += TimerExpiredHandler;
            return timer;
        }

        private void TimerExpiredHandler(object sender, ElapsedEventArgs args)
        {
            var correlationId = string.Empty;
            lock (_lockObject)
            {
                var dictionaryElement = _idTimerMap.FirstOrDefault(kvpair => kvpair.Value == sender);

                if (dictionaryElement.Key != null)
                {
                    correlationId = dictionaryElement.Key;
                }
                var expiredEvent = new TimeoutElapsedEvent
                    {
                        CorrelationId = correlationId,
                        ElapsesMs = (int) dictionaryElement.Value.Interval,
                    };

                try
                {
                    _timeoutCallBack(expiredEvent);
                }
                finally
                {
                    _idExpiresMap.Remove(correlationId);
                    _idTimerMap.Remove(correlationId);
                }
            }
        }

        private static DateTime CalculateExpiration(int timeToElapseInMs)
        {
            return SystemTime.GetTime().AddMilliseconds(timeToElapseInMs);
        }
    }
}
