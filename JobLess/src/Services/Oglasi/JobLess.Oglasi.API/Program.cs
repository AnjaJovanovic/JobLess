using JobLess.Oglasi.Application.Commands.KreirajOglas;
using JobLess.Oglasi.Application.Interfaces;
using JobLess.Shared.Domain.Common;
using JobLess.Oglasi.Application.Interfaces;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using JobLess.Oglasi.Infrastructure.Persistence;
using JobLess.Shared.Domain.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Registracija servisa

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5174") // ili "*"
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// DB Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Ostali servisi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(KreirajOglasCommand).Assembly));
builder.Services.AddScoped<IValidationExceptionThrower, ValidationExceptionThrower>();

// 2️⃣ Build aplikacije
var app = builder.Build();

// 3️⃣ Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();