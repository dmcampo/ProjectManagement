using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Exceptions;
using Xunit;

namespace ProjectManagement.UnitTests.ApplicationTests
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task CreateTask_WithDuplicateOrder_ShouldFail()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var mockTaskRepo = new Mock<ITaskRepository>();
            var mockProjRepo = new Mock<IProjectRepository>();

            var project = new Project { Id = projectId };
            mockProjRepo.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);

            var existingTasks = new List<TaskItem>
            {
                new TaskItem { ProjectId = projectId, Order = 5 }
            };
            mockTaskRepo.Setup(r => r.GetTasksByProjectIdAsync(projectId)).ReturnsAsync(existingTasks);

            var service = new TaskService(mockTaskRepo.Object, mockProjRepo.Object);

            var dto = new CreateTaskDto { Title = "New task", Order = 5 };

            // Act
            Func<Task> act = async () => await service.CreateTaskAsync(projectId, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Order 5 is already in use for this project.");
        }
    }

    public class ProjectServiceTests
    {
        [Fact]
        public async Task DeleteProject_ShouldBeDelete()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId };

            var mockProjRepo = new Mock<IProjectRepository>();
            mockProjRepo.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
            mockProjRepo.Setup(r => r.DeleteAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

            var service = new ProjectService(mockProjRepo.Object);

            // Act
            await service.DeleteProjectAsync(projectId);

            // Assert
            mockProjRepo.Verify(r => r.DeleteAsync(project), Times.Once);
        }
    }
}
