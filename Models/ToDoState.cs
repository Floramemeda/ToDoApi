namespace ToDoApi.Models
{
    /// <summary>
    /// State of the ToDo item
    /// </summary>
    public enum ToDoState
    {
        /// <summary>
        /// Default Value
        /// When the ToDo item isn't processed
        /// </summary>
        ToDo,

        /// <summary>
        /// Default Value
        /// When the ToDo item is in progress
        /// </summary>
        Doing,

        /// <summary>
        /// Default Value
        /// When the ToDo item is completed
        /// </summary>
        Done
    }
}