using ClientEntity = JobLess.Client.Domain.Entities.Client;
using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Entities;
using JobLess.Client.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

using JobLess.Contracts.Events;
using MassTransit;

namespace JobLess.Client.Application.Clients.Commands.ApplyToJob;

public class ApplyToJobCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    : IRequestHandler<ApplyToJobCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(ApplyToJobCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == request.ClientId, cancellationToken);

        if (client is null)
            throw new KeyNotFoundException("Klijent nije pronađen.");

        var alreadyApplied = await context.JobApplications
            .AnyAsync(
                a => a.ClientId == request.ClientId && a.AdvertisementId == request.AdvertisementId,
                cancellationToken);

        if (alreadyApplied)
            throw new InvalidOperationException("Već ste prijavljeni na ovaj oglas.");

        var application = new JobApplication
        {
            ClientId = request.ClientId,
            AdvertisementId = request.AdvertisementId,
            AppliedAt = DateTime.UtcNow,
            Status = JobApplicationStatus.Pending
        };

        context.JobApplications.Add(application);
        await context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.CompanyEmail))
        {
            await publishEndpoint.Publish(
                new JobAppliedMessage(
                    request.AdvertisementId,
                    request.ClientId,
                    client.Email,
                    client.FirstName,
                    client.LastName,
                    request.CompanyEmail),
                cancellationToken);
        }

        return ToDto(application);
    }

    private static JobApplicationDto ToDto(JobApplication application) =>
        new(
            application.ApplicationId,
            application.ClientId,
            application.AdvertisementId,
            application.AppliedAt,
            application.Status);
}
