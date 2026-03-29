using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Application.Services
{
    public class ProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<(IEnumerable<ProjectDto> Projects, int TotalCount)> GetProjectsAsync(ProjectStatus? status, int page, int pageSize, Guid userId)
        {
            var (projects, totalCount) = await _projectRepository.GetProjectsAsync(status, page, pageSize, userId);
            
            var dtos = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status
            }).ToList();

            return (dtos, totalCount);
        }

        public async Task<ProjectSummaryDto> GetProjectSummaryAsync(Guid id, Guid userId)
        {
            var project = await _projectRepository.GetByIdWithTasksAsync(id, userId);
            if (project == null) throw new DomainException("Project not found or you don't have access.");

            return new ProjectSummaryDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                TotalTasks = project.Tasks.Count,
                CompletedTasks = project.Tasks.Count(t => t.IsCompleted)
            };
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, Guid userId)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = ProjectStatus.Draft,
                CreatedByUserId = userId
            };

            await _projectRepository.AddAsync(project);

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status
            };
        }

        public async Task UpdateProjectAsync(Guid id, UpdateProjectDto dto, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(id, userId);
            if (project == null) throw new DomainException("Project not found.");

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.UpdatedAt = DateTime.UtcNow;

            await _projectRepository.UpdateAsync(project);
        }

        public async Task DeleteProjectAsync(Guid id, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(id, userId);
            if (project == null) throw new DomainException("Project not found.");

            await _projectRepository.DeleteAsync(project);
        }

        public async Task ActivateProjectAsync(Guid id, Guid userId)
        {
            var project = await _projectRepository.GetByIdWithTasksAsync(id, userId);
            if (project == null) throw new DomainException("Project not found.");

            project.Activate();
            await _projectRepository.UpdateAsync(project);
        }

        public async Task CompleteProjectAsync(Guid id, Guid userId)
        {
            var project = await _projectRepository.GetByIdWithTasksAsync(id, userId);
            if (project == null) throw new DomainException("Project not found.");

            project.Complete();
            await _projectRepository.UpdateAsync(project);
        }
    }
}
