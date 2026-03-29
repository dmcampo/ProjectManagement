using System;
using System.Collections.Generic;

namespace ProjectManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: projects owned by this user
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
