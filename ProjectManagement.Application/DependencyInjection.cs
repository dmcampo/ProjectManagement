using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Services;

namespace ProjectManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ProjectService>();
            services.AddScoped<TaskService>();
            services.AddScoped<ProjectManagement.Application.Interfaces.IAuthService, AuthService>();

            return services;
        }
    }
}
