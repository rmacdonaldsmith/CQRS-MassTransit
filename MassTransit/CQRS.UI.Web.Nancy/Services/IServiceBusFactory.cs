namespace CQRS.UI.Web.Nancy.Services
{
    public interface IServiceBusFactory<out TServiceBusInterface>
    {
        TServiceBusInterface Get();
    }
}