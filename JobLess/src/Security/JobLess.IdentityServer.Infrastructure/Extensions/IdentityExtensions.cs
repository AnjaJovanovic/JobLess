
using System.Text;
using JobLess.IdentityServer.Application.Interfaces;
using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Infrastructure.Persistence;
using JobLess.IdentityServer.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using JobLess.Contracts.Events;

namespace JobLess.IdentityServer.Infrastructure.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddInfrastructure (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityContext> (options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnectionString"));


            });
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();

        var jwtSection = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
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

                    cfg.Message<UserRegisteredMessage>(
                        m => m.SetEntityName("jobless-user-registered"));

                    cfg.Publish<UserRegisteredMessage>(p => p.ExchangeType = "fanout");
           }); 
        });
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
        }
    };
}