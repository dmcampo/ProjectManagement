using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        [HttpPost]
        public async Task<IActionResult> Create(Guid projectId, CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Validation failed for new task.";
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            try
            {
                await _taskService.CreateTaskAsync(projectId, dto, GetUserId());
                TempData["Success"] = "Task created successfully.";
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Guid projectId, UpdateTaskDto dto)
        {
            try
            {
                await _taskService.UpdateTaskAsync(id, dto, GetUserId());
                TempData["Success"] = "Task updated successfully.";
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Guid projectId)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id, GetUserId());
                TempData["Success"] = "Task deleted successfully.";
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        [HttpPost]
        public async Task<IActionResult> Complete(Guid id, Guid projectId)
        {
            try
            {
                await _taskService.CompleteTaskAsync(id, GetUserId());
                TempData["Success"] = "Task completed successfully.";
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        [HttpPost]
        public async Task<IActionResult> Reorder(Guid id, Guid projectId, string direction)
        {
            try
            {
                await _taskService.ReorderTaskAsync(id, direction, GetUserId());
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
    }
}
