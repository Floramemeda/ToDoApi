
using ToDoApi.ServiceBus.Events;
using System.Threading.Tasks;

namespace ToDoApi.Interfaces
{
    public interface IToDoEventService
    {
        Task PublishEventsThroughEventBusAsync(ToDoItemEvent @event);
        Task AddAndSaveEventAsync(ToDoItemEvent @event);
    }
}