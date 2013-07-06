using EventStore;
using EventStore.Dispatcher;
using MassTransit;
using log4net;

namespace MHM.WinFlexOne.CQRS.CommandDispatch
{
    public class DispatchCommitsToMassTransit : IDispatchCommits
    {
        private readonly ILog m_logger;
        private readonly IServiceBus m_bus;

        public DispatchCommitsToMassTransit(IServiceBus bus, ILog logger)
        {
            m_bus = bus;
            m_logger = logger;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //nothing to do
        }

        /// <summary>
        /// Dispatches the commit specified to the messaging infrastructure.
        /// </summary>
        /// <param name="commit">The commmit to be dispatched.</param>
        public void Dispatch(Commit commit)
        {
            m_logger.InfoFormat("EventStore: sending events to event queue.");
            foreach (var eventMessage in commit.Events)
            {
                m_bus.Publish(eventMessage.Body);
            }
        }
    }
}
