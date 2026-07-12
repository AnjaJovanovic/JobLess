using JobLess.Contracts.Events;
using JobLess.Grpc.Contracts;
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
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        services.AddDbContext<JobApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IJobApplicationDbContext>(sp => sp.GetRequiredService<JobApplicationDbContext>());
        services.AddHttpContextAccessor();

        var clientBaseAddress = configuration["ServiceEndpoints:Client"] ?? "http://localhost:5264";
        services.AddGrpcClient<ClientProfileGrpc.ClientProfileGrpcClient>(options =>
        {
            options.Address = new Uri(clientBaseAddress);
        });
        services.AddScoped<IClientProfileLookupService, ClientProfileGrpcLookupService>();

        var companyBaseAddress = configuration["ServiceEndpoints:Company"] ?? "http://localhost:5288";
        services.AddGrpcClient<CompanyProfileGrpc.CompanyProfileGrpcClient>(options =>
        {
            options.Address = new Uri(companyBaseAddress);
        });
        services.AddScoped<ICompanyLookupService, CompanyProfileGrpcLookupService>();

        services.AddHttpClient<IAdvertisementLookupService, AdvertisementLookupService>(client =>
        {
            client.BaseAddress = new Uri(configuration["ServiceEndpoints:Advertisement"] ?? "http://localhost:5104");
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

                cfg.Message<ApplicationStatusChangedMessage>(
                    m => m.SetEntityName("jobless-application-status-changed"));
                cfg.Publish<ApplicationStatusChangedMessage>(p => p.ExchangeType = "fanout");
            });
        });

        return services;
    }
}
