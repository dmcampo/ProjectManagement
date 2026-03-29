using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProjectStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (projects, totalCount) = await _projectService.GetProjectsAsync(status, page, pageSize, GetUserId());
            return Ok(new
            {
                Items = projects,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetTasks(Guid id, [FromServices] TaskService taskService)
        {
            var tasks = await taskService.GetTasksByProjectIdAsync(id);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            var project = await _projectService.CreateProjectAsync(dto, GetUserId());
            return CreatedAtAction(nameof(Search), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
        {
            await _projectService.UpdateProjectAsync(id, dto, GetUserId());
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _projectService.DeleteProjectAsync(id, GetUserId());
            return NoContent();
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(Guid id)
        {
            var summary = await _projectService.GetProjectSummaryAsync(id, GetUserId());
            return Ok(summary);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _projectService.ActivateProjectAsync(id, GetUserId());
            return NoContent();
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            await _projectService.CompleteProjectAsync(id, GetUserId());
            return NoContent();
        }
    }
}
