using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Net.Models;

namespace WebApi_Net.Controllers
{
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskContext _context;

        public TaskController(TaskContext context)
        {
            _context = context;
        }


        // GET /task
        [HttpGet]
        public async Task<TaskItem[]> GetAll()
        {
            return await _context.TaskItems.ToArrayAsync();
        }


        // GET /task/F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            // parse guid
            var parseFlag = Guid.TryParse(id, out var guid);

            if (!parseFlag)
            {
                // http 400 status
                return BadRequest();
            }

            var taskItem = await _context.TaskItems.FindAsync(guid);

            if (taskItem == null)
            {
                // http 404 status
                return NotFound();
            }

            // http 200 status
            return Ok(new
            {
                Status = taskItem.Status,
                Timestamp = taskItem.Timestamp,
            });
        }


        // POST /task
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            // new task item
            var taskItem = new TaskItem()
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Status = "created",
            };

            // add to db
            await _context.TaskItems.AddAsync(taskItem);
            await _context.SaveChangesAsync();

            // task<task>
            Response.OnCompleted(() => { return Task.Factory.StartNew(async () => await RunAndFinish(taskItem)); });
            return Accepted(taskItem.Id);
        }


        // background run task item
        private async Task RunAndFinish(TaskItem taskItem)
        {
            // new db context
            var context = new TaskContext();

            taskItem.Status = "running";
            taskItem.Timestamp = DateTime.Now;

            context.TaskItems.Update(taskItem);
            await context.SaveChangesAsync();

            await Task.Delay(TimeSpan.FromMinutes(2));

            taskItem.Status = "finished";
            taskItem.Timestamp = DateTime.Now;

            context.TaskItems.Update(taskItem);
            await context.SaveChangesAsync();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // parse guid
            var parseFlag = Guid.TryParse(id, out var guid);

            if (!parseFlag)
            {
                // http 400 status
                return BadRequest();
            }

            var taskItem = await _context.TaskItems.FindAsync(guid);

            if (taskItem == null)
            {
                // http 404 status
                return NotFound();
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            // http 204 status
            return NoContent();
        }
    }
}