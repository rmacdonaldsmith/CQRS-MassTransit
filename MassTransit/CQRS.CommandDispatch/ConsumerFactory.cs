using System;
using System.Collections.Generic;
using CQRS.Commands;
using CQRS.Common;
using CQRS.Domain.Election;
using MassTransit;
using MassTransit.Pipeline;

namespace CQRS.CommandDispatch
{
    public class ConsumerFactory<T> : IConsumerFactory<T> where T : class
    {
        private readonly Dictionary<Type, Type> _messageHandlerMap  = new Dictionary<Type, Type>
                {
                    {typeof (MakeAnElection), typeof(MakeAnElectionCommandHandler)},
                    {typeof(TerminateElection), typeof(TerminateElectionCommandHandler)}
                };

        public ConsumerFactory(ICommandDispatcher commandDispatcher)
        {
            //scan an assembly and get all types that implement the interface Handles<T>
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(
            IConsumeContext<TMessage> context, InstanceHandlerSelector<T, TMessage> selector) 
            where TMessage : class
        {
            //lookup TMessage against all the types we loaded from the assembly we scanned and get back the handler
            if (!_messageHandlerMap.ContainsKey(typeof(T)))
                throw new InvalidOperationException(string.Format("There is no consumer registered to handle commands of type '{0}'.", typeof(T)));

            var consumerType = _messageHandlerMap[typeof(TMessage)];

            var consumerInstance = (T) Activator.CreateInstance(consumerType);

            IEnumerable<Action<IConsumeContext<TMessage>>> result = selector(consumerInstance, context);

            return result;
        }
    }
}
