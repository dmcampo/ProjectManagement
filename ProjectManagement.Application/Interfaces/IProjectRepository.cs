using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Interfaces
{
    public interface IProjectRepository
    {
        Task<(IEnumerable<Project> Projects, int TotalCount)> GetProjectsAsync(ProjectStatus? status, int page, int pageSize, Guid userId);
        Task<Project?> GetByIdAsync(Guid id, Guid userId);
        Task<Project?> GetByIdWithTasksAsync(Guid id, Guid userId);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Project project);
    }
}
