using System;
using ToDoApi.Models;

namespace ToDoApi.ServiceBus.Events
{
    /// <summary>
    /// Event for when a ToDo item is updated
    /// </summary>
    public class ToDoItemChangedEvent : ToDoItemEvent
    {
        /// <summary>
        /// Id of the ToDo item
        /// </summary>
        /// <value></value>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the ToDo item
        /// </summary>
        /// <value></value>
        public string Title { get; set; }

        /// <summary>
        /// State of the ToDo item
        /// </summary>
        /// <value></value>
        public ToDoState State { get; set; }

        /// <summary>
        /// Constructor for the event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="state"></param>
        public ToDoItemChangedEvent(Guid id, string title, ToDoState state)
        {
            Id = id;
            Title = title;
            State = state;
        }
    }
}
