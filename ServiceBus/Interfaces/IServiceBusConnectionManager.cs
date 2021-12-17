using Microsoft.Azure.ServiceBus;

namespace ToDoApi.ServiceBus.Interfaces
{
    public interface IServiceBusConnectionManager
    {
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        ITopicClient CreateTopicClient();
    }
}