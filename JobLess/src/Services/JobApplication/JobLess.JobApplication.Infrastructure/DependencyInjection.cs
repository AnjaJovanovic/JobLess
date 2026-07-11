using JobLess.Contracts.Events;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Infrastructure.Persistence;
using JobLess.JobApplication.Infrastructure.Services;
using MassTransit;
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
        services.AddHttpClient<IClientProfileLookupService, ClientProfileLookupService>(client =>
        {
            client.BaseAddress = new Uri(configuration["ServiceEndpoints:Client"] ?? "http://localhost:5263");
        });

        services.AddHttpClient<ICompanyLookupService, CompanyLookupService>(client =>
        {
            client.BaseAddress = new Uri(configuration["ServiceEndpoints:Company"] ?? "http://localhost:5287");
        });

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                var rabbitHost = configuration["RabbitMq:Host"] ?? "localhost";
                var rabbitUser = configuration["RabbitMq:Username"] ?? "guest";
                var rabbitPass = configuration["RabbitMq:Password"] ?? "guest";

                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                });

                cfg.Message<JobAppliedMessage>(
                    m => m.SetEntityName("jobless-job-applied"));
                cfg.Publish<JobAppliedMessage>(p => p.ExchangeType = "fanout");
            });
        });

        return services;
    }
}
