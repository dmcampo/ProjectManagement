using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(Guid projectId);
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<int> GetMaxOrderByProjectIdAsync(Guid projectId);
        Task ReorderTasksAsync(IEnumerable<TaskItem> tasksToUpdate);
    }
}
