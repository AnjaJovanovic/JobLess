using JobLess.Notification.Application.Interfaces;
using JobLess.Notification.Infrastructure.Persistence;
using JobLess.Notification.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobLess.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<INotificationDbContext>(sp => sp.GetRequiredService<NotificationDbContext>());
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
