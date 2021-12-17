using System;
using System.Collections.Generic;
using ToDoApi.ServiceBus.Events;

namespace ToDoApi.ServiceBus.Interfaces
{
    public interface ISubscriptionManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T1, T2>()
           where T1 : ToDoItemEvent
           where T2 : IToDoItemEventHandler<T1>;

        void RemoveSubscription<T1, T2>()
           where T1 : ToDoItemEvent
           where T2 : IToDoItemEventHandler<T1>;

        bool HasSubscriptionsForEvent<T>() where T : ToDoItemEvent;

        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : ToDoItemEvent;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();
    }
}