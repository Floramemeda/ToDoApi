using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDoApi.Models;
using ToDoApi.Repositories;
using ToDoApi.Services;
using ToDoApi.ServiceBus.Events;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Interfaces;

namespace ToDoApi.Controllers
{
    /// <summary>
    /// ToDo Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoContext _context;
        private readonly IToDoEventService _toDoEventService;

        /// <summary>
        /// Constructor for ToDo controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toDoEventService"></param>
        public ToDoController(ToDoContext context, IToDoEventService toDoEventService)
        {
            _context = context;
            _toDoEventService = toDoEventService;
        }

        /// <summary>
        /// Get all ToDos in the database
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<ToDo>), (int)HttpStatusCode.OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetAllToDosAsync()
        {
            var toDos = await _context.ToDos.ToListAsync();
            return Ok(toDos);
        }

        /// <summary>
        /// Get a ToDo item with ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ToDo), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<ToDo>> GetToDoWithIdAsync([FromRoute]Guid id)
        {
            var toDo = await _context.ToDos.SingleOrDefaultAsync(i => i.Id == id);
            if(toDo == null)
            {
                return NotFound($"Failed to find ToDo item with ID: {toDo.Id}");
            }
            else
            {
                return Ok(toDo);
            }

        }

        /// <summary>
        /// Create a new ToDo item
        /// </summary>
        /// <param name="toDo"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ToDo), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost]
        public async Task<ActionResult<ToDo>> CreateToDoItemAsync ([FromBody]ToDoContract toDo)
        {
            ToDo item = new ToDo {Id = Guid.NewGuid(), Title = toDo.Title, State = toDo.State ?? ToDoState.ToDo};
            _context.ToDos.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ToDo), new {id = item.Id}, item);
        }


        /// <summary>
        /// Update existing ToDo item
        /// </summary>
        /// <param name="toDo"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPut]
        public async Task<ActionResult> UpdateToDoItemAsync([FromBody]ToDo toDo)
        {
            var existingToDoItem = await _context.ToDos.SingleOrDefaultAsync(t => t.Id == toDo.Id);

            if(existingToDoItem == null)
            {
                return NotFound($"Failed to find ToDo item with ID: {toDo.Id}");
            }

            else
            {
                if(existingToDoItem.Title == toDo.Title && existingToDoItem.State == toDo.State)
                {
                    return NoContent();
                }
                else
                {
                    var toDoItemChangedEvent = new ToDoItemChangedEvent(toDo.Id, toDo.Title, toDo.State);
                    await _toDoEventService.AddAndSaveEventAsync(toDoItemChangedEvent);
                    await _toDoEventService.PublishEventsThroughEventBusAsync(toDoItemChangedEvent);
                }
            }
            return NoContent();
        }

        /// <summary>
        /// Delete an existing ToDo item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ToDo), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete]
        public async Task<ActionResult> DeleteToDoItemAsync(Guid id)
        {
            var existingToDoItem = await _context.ToDos.SingleOrDefaultAsync(t => t.Id == id);
            if(existingToDoItem == null)
            {
                return NotFound($"Failed to find ToDo item with ID: {id}");
            }
            else
            {
                _context.ToDos.Remove(existingToDoItem);
                await _context.SaveChangesAsync();
                return Ok($"Successfully deleted ToDo item with ID {id}");
            }
        }
        
    }
}