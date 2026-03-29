using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

        // 3NF: FK to User who created this project
        public Guid CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        // Audit timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public void Activate()
        {
            if (Status != ProjectStatus.Draft)
            {
                throw new DomainException("Only Draft projects can be activated.");
            }

            if (!Tasks.Any())
            {
                throw new DomainException("A project can only be activated if it has at least one task.");
            }

            Status = ProjectStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != ProjectStatus.Active)
            {
                throw new DomainException("Only Active projects can be completed.");
            }

            if (Tasks.Any(t => !t.IsCompleted))
            {
                throw new DomainException("A project can only be completed if all its tasks are completed.");
            }

            Status = ProjectStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
