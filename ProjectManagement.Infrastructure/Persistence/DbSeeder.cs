using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
        {
            // 1. Seed Admin User
            var adminEmail = "admin@admin.com";
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = adminEmail,
                    Name = "Admin",
                    PasswordHash = passwordHasher.HashPassword("admin123"),
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }

            // 2. Seed Projects if none exist for this admin
            if (!await context.Projects.AnyAsync(p => p.CreatedByUserId == adminUser.Id))
            {
                var projects = new List<Project>
                {
                    new Project
                    {
                        Id = Guid.NewGuid(),
                        Name = "Rediseño de E-commerce",
                        Description = "Proyecto para modernizar la interfaz de la tienda online usando Clean Architecture.",
                        Status = ProjectStatus.Active,
                        CreatedByUserId = adminUser.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Tasks = new List<TaskItem>
                        {
                            new TaskItem { Title = "Diseñar Maquetas", Priority = TaskPriority.High, Order = 1, IsCompleted = true, CreatedAt = DateTime.UtcNow },
                            new TaskItem { Title = "Implementar API de Productos", Priority = TaskPriority.Medium, Order = 2, IsCompleted = false, CreatedAt = DateTime.UtcNow },
                            new TaskItem { Title = "Configurar Pasarela de Pagos", Priority = TaskPriority.High, Order = 3, IsCompleted = false, CreatedAt = DateTime.UtcNow }
                        }
                    },
                    new Project
                    {
                        Id = Guid.NewGuid(),
                        Name = "App de Logística",
                        Description = "Gestión de flotas y seguimiento en tiempo real de pedidos.",
                        Status = ProjectStatus.Draft,
                        CreatedByUserId = adminUser.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Tasks = new List<TaskItem>
                        {
                            new TaskItem { Title = "Definir Arquitectura", Priority = TaskPriority.High, Order = 1, IsCompleted = false, CreatedAt = DateTime.UtcNow }
                        }
                    }
                };

                context.Projects.AddRange(projects);
                await context.SaveChangesAsync();
            }
        }
    }
}
