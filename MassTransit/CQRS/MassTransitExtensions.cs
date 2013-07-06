using System;
using System.Threading;
using MassTransit;

namespace MHM.WinFlexOne.CQRS
{
    public static class MassTransitExtensions
    {
        public static void RequestResponse<TRequest, TResponse>(this IServiceBus bus, TRequest command, Action<TResponse> responseHandlerDelegate, Action handleTimeoutDelegate, TimeSpan timeOut)
            where TRequest : class where TResponse : class
        {
            var t = new Thread(() => PerformRequest(bus, command, responseHandlerDelegate, handleTimeoutDelegate, timeOut));
            t.Start();
            t.Join();
        }

        private static void PerformRequest<TRequest, TResponse>(IServiceBus bus, TRequest command, Action<TResponse> responseHandlerDelegate, Action handleTimeoutDelegate, TimeSpan timeOut) 
            where TRequest : class where TResponse : class
        {
            bus.PublishRequest(command, requestConfig =>
            {
                //requestConfig.UseCurrentSynchronizationContext();
                requestConfig.Handle<TResponse>(message => responseHandlerDelegate(message));
                requestConfig.HandleTimeout(timeOut, () => handleTimeoutDelegate.Invoke());
                //requestConfig.SetTimeout(timeOut);
            });
        }
    }
}
