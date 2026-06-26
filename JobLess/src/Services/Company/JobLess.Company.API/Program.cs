using JobLess.Company.Application.Commands.Create;
using JobLess.Company.Application.Common.Behaviors;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Infrastructure.Persistence;
using JobLess.Shared.Domain.Common;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

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

builder.Services.AddValidatorsFromAssemblyContaining<CreateCompanyCommand>();



builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IValidationExceptionThrower, ValidationExceptionThrower>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error?.Error is ValidationException validationEx)
        {
            var messages = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(new { errors = messages });
            await context.Response.WriteAsync(json);
        }
        else
        {
            var json = System.Text.Json.JsonSerializer.Serialize(new { errors = new[] { "Doùlo je do greùke." } });
            await context.Response.WriteAsync(json);
        }
    });
});

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();
}

app.Run();