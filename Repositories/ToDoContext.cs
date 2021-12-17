using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;
using ToDoApi.ServiceBus.Events;

namespace ToDoApi.Repositories
{
    /// <summary>
    /// Database context of ToDo
    /// </summary>
    public class ToDoContext : DbContext
    {
        /// <summary>
        /// Database context of ToDo
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {

        }
        /// <summary>
        /// DB set of ToDo items
        /// </summary>
        /// <value></value>
        public DbSet<ToDo> ToDos { get; set; }

        public DbSet<EventLogEntry> EventLogEntries { get; set; }
    }
}