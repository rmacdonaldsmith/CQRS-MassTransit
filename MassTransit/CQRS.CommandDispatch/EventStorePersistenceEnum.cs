namespace CQRS.CommandDispatch
{
    public enum EventStorePersistenceEnum
    {
        InMemory,
        MongoDb,
        SqlServer
    }
}
