using ToDoApi.Interfaces;
using ToDoApi.Repositories;
using ToDoApi.ServiceBus.Events;
using ToDoApi.ServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ToDoApi.Services
{
    public class ToDoEventService : IToDoEventService
    {
        private readonly ToDoContext _toDoContext;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ToDoEventService> _logger;
        private readonly Func<DbConnection, IEventLogService> _eventLogServiceFactory;
        private readonly IEventLogService _eventLogService;

        public ToDoEventService(ToDoContext toDoContext, Func<DbConnection, IEventLogService> eventLogServiceFactory,
                                     IEventBus eventBus,
                                     ILogger<ToDoEventService> logger)
        {
            _toDoContext = toDoContext ?? throw new ArgumentNullException(nameof(toDoContext));
            _eventLogServiceFactory = eventLogServiceFactory ?? throw new ArgumentNullException(nameof(eventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _eventLogServiceFactory(toDoContext.Database.GetDbConnection());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAndSaveEventAsync(ToDoItemEvent @event)
        {
            await ResilientTransaction.CreateNew(_toDoContext).ExecuteAsync(async () =>
            {
                await _toDoContext.SaveChangesAsync();
                await _eventLogService.SaveEventAsync(@event, _toDoContext.Database.CurrentTransaction);
            });
        }

        public async Task PublishEventsThroughEventBusAsync(ToDoItemEvent @event)
        {
            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(@event.Id);
                await _eventBus.PublishAsync(@event);
                await _eventLogService.MarkEventAsPublishedAsync(@event.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR publishing integration event: '{ToDoItemEventId}'", @event.Id);

                await _eventLogService.MarkEventAsFailedAsync(@event.Id);
            }
        }
    }
}