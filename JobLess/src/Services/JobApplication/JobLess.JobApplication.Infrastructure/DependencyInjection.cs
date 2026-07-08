using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobLess.JobApplication.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<JobApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IJobApplicationDbContext>(sp => sp.GetRequiredService<JobApplicationDbContext>());

        return services;
    }
}
