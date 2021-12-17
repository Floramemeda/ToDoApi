using ToDoApi.ServiceBus.Events;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ToDoApi.ServiceBus.Events
{
    /// <summary>
    /// An entry to the event log
    /// </summary>
    public class EventLogEntry
    {
        private EventLogEntry() { }

        /// <summary>
        /// Ctor
        /// </summary>
        public EventLogEntry(ToDoItemEvent @event, Guid transactionId)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStates.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
        }

        /// <summary>
        /// Event Id
        /// </summary>
        /// <value></value>
        [Required]
        [Key]
        public Guid EventId { get; private set; }

        /// <summary>
        /// Event Type Name
        /// </summary>
        /// <value></value>
        public string EventTypeName { get; private set; }

        /// <summary>
        /// Event Type Short Name
        /// </summary>
        /// <returns></returns>
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();

        /// <summary>
        /// Integrati
        /// </summary>
        /// <value></value>
        [NotMapped]
        public ToDoItemEvent ToDoItemEvent { get; private set; }

        /// <summary>
        /// State of the Event
        /// </summary>
        /// <value></value>
        public EventStates State { get; set; }

        /// <summary>
        /// When the event is sent
        /// </summary>
        /// <value></value>
        public int TimesSent { get; set; }

        /// <summary>
        /// Timestamp of creation
        /// </summary>
        /// <value></value>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// Content of event
        /// </summary>
        /// <value></value>
        public string Content { get; private set; }

        /// <summary>
        /// Transaction ID
        /// </summary>
        /// <value></value>
        public string TransactionId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public EventLogEntry DeserializeJsonContent(Type type)
        {
            ToDoItemEvent = JsonConvert.DeserializeObject(Content, type) as ToDoItemEvent;
            return this;
        }
    }
}