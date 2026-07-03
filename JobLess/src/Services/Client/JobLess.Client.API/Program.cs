using System.Text;
using JobLess.Client.Application;
using JobLess.Client.Application.Interfaces;
using JobLess.Client.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using JobLess.Contracts.Events;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

builder.Services.AddApplication();

var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
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

builder.Services.AddAuthorization();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var rabbitUser = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var rabbitPass = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.Message<JobAppliedMessage>(m => m.SetEntityName("jobless-job-applied"));
        cfg.Publish<JobAppliedMessage>(p => p.ExchangeType = "fanout");
        
        cfg.Message<ApplicationStatusChangedMessage>(m => m.SetEntityName("jobless-application-status-changed"));
        cfg.Publish<ApplicationStatusChangedMessage>(p => p.ExchangeType = "fanout");
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "JobLess.Client.API",
        Version = "v1",
        Description = "API za upravljanje profilom korisnika (klijenta)."
    });

    options.UseInlineDefinitionsForEnums();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "JobLess.Client.API v1");
});

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
