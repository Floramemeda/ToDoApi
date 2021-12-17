using System.ComponentModel.DataAnnotations;
using System;

namespace ToDoApi.Models
{
    /// <summary>
    /// Model of a ToDo item
    /// </summary>
    public class ToDo
    {
        /// <summary>
        /// Id of the ToDo item
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the ToDo item
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// State of the ToDo item
        /// </summary>
        public ToDoState State { get; set; }
    }
}