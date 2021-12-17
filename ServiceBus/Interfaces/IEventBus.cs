using System.Threading.Tasks;
using ToDoApi.ServiceBus.Events;

namespace ToDoApi.ServiceBus.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync(ToDoItemEvent @event);

        Task SubscribeAsync<T, TH>()
            where T : ToDoItemEvent
            where TH : IToDoItemEventHandler<T>;

        Task UnsubscribeAsync<T, TH>()
            where TH : IToDoItemEventHandler<T>
            where T : ToDoItemEvent;

        Task SetupAsync();
    }
}