using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Application.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<TaskItemDto>> GetTasksByProjectIdAsync(Guid projectId)
        {
            // Internal use (when project is already validated). For external, we should validate. 
            // In ProjectsController.Details it is already validated via ProjectService.
            var tasks = await _taskRepository.GetTasksByProjectIdAsync(projectId);
            return tasks.Select(t => new TaskItemDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Priority = t.Priority,
                Order = t.Order,
                IsCompleted = t.IsCompleted
            });
        }

        public async Task<TaskItemDto> CreateTaskAsync(Guid projectId, CreateTaskDto dto, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId, userId);
            if (project == null) throw new DomainException("Project not found or access denied.");

            int orderToAssign;
            if (dto.Order.HasValue)
            {
                var existingTasks = await _taskRepository.GetTasksByProjectIdAsync(projectId);
                if (existingTasks.Any(t => t.Order == dto.Order.Value))
                {
                    throw new DomainException($"Order {dto.Order.Value} is already in use for this project.");
                }
                orderToAssign = dto.Order.Value;
            }
            else
            {
                var maxOrder = await _taskRepository.GetMaxOrderByProjectIdAsync(projectId);
                orderToAssign = maxOrder + 1;
            }

            var taskItem = new TaskItem
            {
                ProjectId = projectId,
                Title = dto.Title,
                Priority = dto.Priority,
                Order = orderToAssign,
                IsCompleted = false
            };

            await _taskRepository.AddAsync(taskItem);

            return new TaskItemDto
            {
                Id = taskItem.Id,
                ProjectId = taskItem.ProjectId,
                Title = taskItem.Title,
                Priority = taskItem.Priority,
                Order = taskItem.Order,
                IsCompleted = taskItem.IsCompleted
            };
        }

        public async Task UpdateTaskAsync(Guid id, UpdateTaskDto dto, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) throw new DomainException("Task not found.");

            var project = await _projectRepository.GetByIdAsync(task.ProjectId, userId);
            if (project == null) throw new DomainException("Access denied.");

            task.Title = dto.Title;
            task.Priority = dto.Priority;

            await _taskRepository.UpdateAsync(task);
        }

        public async Task DeleteTaskAsync(Guid id, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) throw new DomainException("Task not found.");

            var project = await _projectRepository.GetByIdAsync(task.ProjectId, userId);
            if (project == null) throw new DomainException("Access denied.");

            var projectId = task.ProjectId;
            await _taskRepository.DeleteAsync(task);

            // Recompact remaining orders
            var remainingTasks = (await _taskRepository.GetTasksByProjectIdAsync(projectId)).ToList();
            var order = 1;
            bool updated = false;
            foreach (var t in remainingTasks)
            {
                if (t.Order != order)
                {
                    t.Order = order;
                    updated = true;
                }
                order++;
            }

            if (updated)
            {
                await _taskRepository.ReorderTasksAsync(remainingTasks);
            }
        }

        public async Task CompleteTaskAsync(Guid id, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) throw new DomainException("Task not found.");

            var project = await _projectRepository.GetByIdAsync(task.ProjectId, userId);
            if (project == null) throw new DomainException("Access denied.");

            task.IsCompleted = true;
            await _taskRepository.UpdateAsync(task);
        }

        public async Task ReorderTaskAsync(Guid id, string direction, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) throw new DomainException("Task not found.");

            var project = await _projectRepository.GetByIdAsync(task.ProjectId, userId);
            if (project == null) throw new DomainException("Access denied.");

            var allTasks = (await _taskRepository.GetTasksByProjectIdAsync(task.ProjectId)).ToList();
            var currentIndex = allTasks.FindIndex(t => t.Id == id);

            if (direction.Equals("up", StringComparison.OrdinalIgnoreCase))
            {
                if (currentIndex > 0)
                {
                    var prevTask = allTasks[currentIndex - 1];
                    (task.Order, prevTask.Order) = (prevTask.Order, task.Order);
                    await _taskRepository.ReorderTasksAsync(new[] { task, prevTask });
                }
            }
            else if (direction.Equals("down", StringComparison.OrdinalIgnoreCase))
            {
                if (currentIndex < allTasks.Count - 1)
                {
                    var nextTask = allTasks[currentIndex + 1];
                    (task.Order, nextTask.Order) = (nextTask.Order, task.Order);
                    await _taskRepository.ReorderTasksAsync(new[] { task, nextTask });
                }
            }
            else
            {
                throw new DomainException("Invalid direction.");
            }
        }
    }
}
