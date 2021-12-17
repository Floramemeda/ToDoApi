using ToDoApi.Repositories;
using ToDoApi.ServiceBus.Events;
using ToDoApi.ServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ToDoApi.Services
{
    public class EventLogService : IEventLogService
    {
        private readonly ToDoContext _toDoContext;
        private readonly DbConnection _dbConnection;
        private readonly List<Type> _eventTypes;

        public EventLogService(DbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _toDoContext = new ToDoContext(
                new DbContextOptionsBuilder<ToDoContext>()
                    .UseSqlServer(_dbConnection).Options);

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(ToDoItemEvent)))
                .ToList();
        }

        public async Task<IEnumerable<EventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            return await _toDoContext.EventLogEntries
                .Where(e => e.TransactionId == tid && e.State == EventStates.NotPublished)
                .OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)))
                .ToListAsync();
        }

        public Task SaveEventAsync(ToDoItemEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new EventLogEntry(@event, transaction.TransactionId);

            _toDoContext.Database.UseTransaction(transaction.GetDbTransaction());
            _toDoContext.EventLogEntries.Add(eventLogEntry);

            return _toDoContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStates.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStates.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStates.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStates status)
        {
            var eventLogEntry = _toDoContext.EventLogEntries.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStates.InProgress)
                eventLogEntry.TimesSent++;

            _toDoContext.EventLogEntries.Update(eventLogEntry);

            return _toDoContext.SaveChangesAsync();
        }
    }
}