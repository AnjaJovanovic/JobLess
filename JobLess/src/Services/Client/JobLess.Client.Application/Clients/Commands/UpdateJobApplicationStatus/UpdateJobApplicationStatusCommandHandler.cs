using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Entities;
using JobLess.Client.Domain.Enums;
using JobLess.Contracts.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Commands.UpdateJobApplicationStatus;

public class UpdateJobApplicationStatusCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    : IRequestHandler<UpdateJobApplicationStatusCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(
        UpdateJobApplicationStatusCommand request,
        CancellationToken cancellationToken)
    {
        var application = await context.JobApplications
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId, cancellationToken);

        if (application is null)
            throw new KeyNotFoundException("Prijava nije pronađena.");

        application.Status = request.Status;
        await context.SaveChangesAsync(cancellationToken);

        if (request.Status is JobApplicationStatus.Accepted or JobApplicationStatus.Rejected)
        {
            var statusLabel = request.Status == JobApplicationStatus.Accepted ? "Accepted" : "Rejected";

            await publishEndpoint.Publish(
                new ApplicationStatusChangedMessage(
                    application.ApplicationId,
                    application.AdvertisementId,
                    application.Client.Email,
                    application.Client.FirstName,
                    application.Client.LastName,
                    statusLabel),
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
