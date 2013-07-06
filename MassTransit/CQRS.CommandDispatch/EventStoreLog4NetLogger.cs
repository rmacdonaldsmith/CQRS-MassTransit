using System;
using log4net;
using ILog = EventStore.Logging.ILog;

namespace MHM.WinFlexOne.CQRS.CommandDispatch
{
    //EventStore.Logging.Log4Net was compiled with a reference to log4net v1.2.10
    //MassTransit.Log4NetIntegration was compiled with a reference to log4net v1.2.11
    //therefore they can not both run in the same process. Hence, I had to write a custom
    //impl for the EventStore logger so that I can control which log4net version to use.
    public class EventStoreLog4NetLogger : ILog
    {
        private readonly log4net.ILog log;

        public EventStoreLog4NetLogger(Type typeToLog)
        {
            this.log = LogManager.GetLogger(typeToLog);
        }

        public virtual void Verbose(string message, params object[] values)
        {
            if (!this.log.IsDebugEnabled)
                return;
            this.log.DebugFormat(message, values);
        }

        public virtual void Debug(string message, params object[] values)
        {
            if (!this.log.IsDebugEnabled)
                return;
            this.log.DebugFormat(message, values);
        }

        public virtual void Info(string message, params object[] values)
        {
            if (!this.log.IsInfoEnabled)
                return;
            this.log.InfoFormat(message, values);
        }

        public virtual void Warn(string message, params object[] values)
        {
            if (!this.log.IsWarnEnabled)
                return;
            this.log.WarnFormat(message, values);
        }

        public virtual void Error(string message, params object[] values)
        {
            if (!this.log.IsErrorEnabled)
                return;
            this.log.ErrorFormat(message, values);
        }

        public virtual void Fatal(string message, params object[] values)
        {
            if (!this.log.IsFatalEnabled)
                return;
            this.log.FatalFormat(message, values);
        }
    }
}
