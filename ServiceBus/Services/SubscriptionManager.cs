using System;
using System.Collections.Generic;
using System.Linq;
using ToDoApi.ServiceBus.Events;
using ToDoApi.ServiceBus.Interfaces;

namespace ToDoApi.ServiceBus.Services
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public SubscriptionManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();

        public void AddSubscription<T1, T2>()
            where T1 : ToDoItemEvent
            where T2 : IToDoItemEventHandler<T1>
        {
            var eventName = GetEventKey<T1>();

            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            var handlerType = typeof(T2);

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type '{handlerType.Name}' already registered for '{eventName}'",
                    nameof(handlerType));
            }

            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));

            if (!_eventTypes.Contains(typeof(T1)))
            {
                _eventTypes.Add(typeof(T1));
            }
        }

        public void Clear() => _handlers.Clear();

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        public Type GetEventTypeByName(string eventName) => _eventTypes
                                                            .SingleOrDefault(t => t.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : ToDoItemEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public bool HasSubscriptionsForEvent<T>() where T : ToDoItemEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public void RemoveSubscription<T1, T2>()
            where T1 : ToDoItemEvent
            where T2 : IToDoItemEventHandler<T1>
        {
            var handlerToRemove = FindSubscriptionToRemove<T1, T2>();
            var eventName = GetEventKey<T1>();
            if (handlerToRemove != null)
            {
                _handlers[eventName].Remove(handlerToRemove);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }

        private SubscriptionInfo FindSubscriptionToRemove<T1, T2>()
             where T1 : ToDoItemEvent
             where T2 : IToDoItemEventHandler<T1>
        {
            var eventName = GetEventKey<T1>();

            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == typeof(T1));
        }
    }
}