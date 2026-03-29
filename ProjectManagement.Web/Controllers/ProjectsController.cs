using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Web.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        public async Task<IActionResult> Index(ProjectStatus? status, int page = 1)
        {
            int pageSize = 10;
            var (projects, totalCount) = await _projectService.GetProjectsAsync(status, page, pageSize, GetUserId());

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(projects);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _projectService.CreateProjectAsync(dto, GetUserId());
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var summary = await _projectService.GetProjectSummaryAsync(id, GetUserId());
                var dto = new UpdateProjectDto { Name = summary.Name, Description = summary.Description };
                ViewBag.ProjectId = id;
                return View(dto);
            }
            catch (DomainException)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProjectId = id;
                return View(dto);
            }

            try
            {
                await _projectService.UpdateProjectAsync(id, dto, GetUserId());
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (DomainException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.ProjectId = id;
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, [FromServices] TaskService taskService)
        {
            try
            {
                var summary = await _projectService.GetProjectSummaryAsync(id, GetUserId());
                var tasks = await taskService.GetTasksByProjectIdAsync(id);

                ViewBag.Tasks = tasks;
                return View(summary);
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id, GetUserId());
                TempData["Success"] = "Project deleted successfully.";
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                await _projectService.ActivateProjectAsync(id, GetUserId());
                TempData["Success"] = "Project activated successfully.";
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Complete(Guid id)
        {
            try
            {
                await _projectService.CompleteProjectAsync(id, GetUserId());
                TempData["Success"] = "Project completed successfully.";
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
