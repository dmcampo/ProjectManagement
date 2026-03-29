using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        [HttpPost("{projectId}")]
        public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto dto)
        {
            var task = await _taskService.CreateTaskAsync(projectId, dto, GetUserId());
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
        {
            await _taskService.UpdateTaskAsync(id, dto, GetUserId());
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await _taskService.DeleteTaskAsync(id, GetUserId());
            return NoContent();
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            await _taskService.CompleteTaskAsync(id, GetUserId());
            return NoContent();
        }

        [HttpPatch("{id}/reorder")]
        public async Task<IActionResult> ReorderTask(Guid id, [FromQuery] string direction)
        {
            await _taskService.ReorderTaskAsync(id, direction, GetUserId());
            return NoContent();
        }
    }
}
