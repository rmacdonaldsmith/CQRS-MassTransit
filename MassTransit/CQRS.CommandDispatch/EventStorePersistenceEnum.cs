namespace MHM.WinFlexOne.CQRS.CommandDispatch
{
    public enum EventStorePersistenceEnum
    {
        InMemory,
        MongoDb,
        SqlServer
    }
}
