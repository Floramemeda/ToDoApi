using System;

namespace ToDoApi.Models
{
    /// <summary>
    /// Contract for creating a ToDo item
    /// </summary>
    public class ToDoContract
    {
        /// <summary>
        /// Title of ToDo item
        /// </summary>
        /// <value></value>
        public string Title { get; set; } 

        /// <summary>
        /// State of ToDo item, defaults to ToDoState.ToDo
        /// </summary>
        /// <value></value>
        public ToDoState? State { get; set; }
    }
}