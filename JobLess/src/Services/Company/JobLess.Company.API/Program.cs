using JobLess.Company.Application.Commands.Create;
using JobLess.Company.Application.Interfaces;
using JobLess.Shared.Domain.Common;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using JobLess.Company.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5174")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCompanyCommand).Assembly));
builder.Services.AddScoped<IValidationExceptionThrower, ValidationExceptionThrower>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.MapControllers();

app.Run();