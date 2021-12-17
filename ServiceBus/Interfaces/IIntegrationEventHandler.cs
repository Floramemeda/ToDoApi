using System.Threading.Tasks;
using ToDoApi.ServiceBus.Events;

namespace ToDoApi.ServiceBus.Interfaces
{
    public interface IToDoItemEventHandler<in TToDoItemEvent>
        where TToDoItemEvent : ToDoItemEvent
    {
        Task HandleAsync(TToDoItemEvent @event);
    }
}