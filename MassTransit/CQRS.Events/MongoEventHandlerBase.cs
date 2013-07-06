using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MongoDB.Driver;
using log4net;

namespace MHM.WinFlexOne.CQRS.Events
{
    public abstract class MongoEventHandlerBase<TEvent, TDto> : Handles<TEvent> where TEvent : IEvent
    {
        private readonly MongoDatabase _mongoDataBase;
        private readonly string _collectionName;
        private readonly Expression<Func<TEvent, TDto>> _mapEventToDtoExpression;
        private readonly ILog _logger = LogManager.GetLogger(typeof(MongoEventHandlerBase<TEvent, TDto>));

        protected MongoEventHandlerBase(MongoDatabase mongoDataBase, string collectionName, Expression<Func<TEvent, TDto>> mapEventToDtoExpression)
        {
            //TODO: instead of taking an expression like this in the constructor, create a Builder that will build the handler from a fluent API.
            _mongoDataBase = mongoDataBase;
            _collectionName = collectionName;
            _mapEventToDtoExpression = mapEventToDtoExpression;
        }

        protected ILog Logger 
        {
            get { return _logger; }
        }

        public virtual void Persist(string collectionName, TDto dtoToPersist)
        {
            var collection = GetOrCreateCollection(collectionName);

            var safeModeResult = collection.Save(dtoToPersist);
            //use the safe mode result to test if the upsert was ok.
            //if the upsert failed, then we can generate a message (containing this event) and put it back on the bus
            //there could be an admin console that monitors this error q so that people know when upserts fail.
        }

        protected MongoCollection<TDto> GetOrCreateCollection(string collectionName)
        {
            if (_mongoDataBase.CollectionExists(collectionName) == false)
            {
                _mongoDataBase.CreateCollection(collectionName);
            }

            return _mongoDataBase.GetCollection<TDto>(collectionName);
        }

        public virtual void Handle(TEvent @event)
        {
            var dto = _mapEventToDtoExpression.Compile()(@event);

            Persist(_collectionName, dto);
        }
    }
}
