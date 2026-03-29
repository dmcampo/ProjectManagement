using System;
using System.Linq;
using FluentAssertions;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Exceptions;
using Xunit;

namespace ProjectManagement.UnitTests.DomainTests
{
    public class ProjectDomainTests
    {
        [Fact]
        public void ActivateProject_WithTasks_ShouldSucceed()
        {
            // Arrange
            var project = new Project { Status = ProjectStatus.Draft };
            project.Tasks.Add(new TaskItem { Title = "Task 1" });

            // Act
            project.Activate();

            // Assert
            project.Status.Should().Be(ProjectStatus.Active);
        }

        [Fact]
        public void ActivateProject_WithoutTasks_ShouldFail()
        {
            // Arrange
            var project = new Project { Status = ProjectStatus.Draft };

            // Act
            Action act = () => project.Activate();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("A project can only be activated if it has at least one task.");
        }

        [Fact]
        public void CompleteProject_WithPendingTasks_ShouldFail()
        {
            // Arrange
            var project = new Project { Status = ProjectStatus.Active };
            project.Tasks.Add(new TaskItem { Title = "Task 1", IsCompleted = true });
            project.Tasks.Add(new TaskItem { Title = "Task 2", IsCompleted = false }); // Pending task

            // Act
            Action act = () => project.Complete();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("A project can only be completed if all its tasks are completed.");
        }
    }
}
