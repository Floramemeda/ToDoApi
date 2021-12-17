using ToDoApi.ServiceBus.Events;
using System.Threading.Tasks;

namespace ToDoApi.ServiceBus.Interfaces
{
    public interface IToDoEventLogService
    {
        Task PublishEventsThroughEventBusAsync(ToDoItemEvent @event);
        Task AddAndSaveEventAsync(ToDoItemEvent @event);
    }
}