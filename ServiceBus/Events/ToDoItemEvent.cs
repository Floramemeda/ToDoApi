using Newtonsoft.Json;
using System;

namespace ToDoApi.ServiceBus.Events
{
    /// <summary>
    /// Event relates to a ToDo item
    /// </summary>
    public abstract class ToDoItemEvent
    {
        /// <summary>
        /// Constructor of an event related to a ToDo item
        /// </summary>
        [JsonConstructor]
        public ToDoItemEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Event ID
        /// </summary>
        /// <value></value>
        [JsonProperty]
        public Guid Id { get; private set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        /// <value></value>
        [JsonProperty]
        public DateTime CreationDate { get; private set; }
    }
}