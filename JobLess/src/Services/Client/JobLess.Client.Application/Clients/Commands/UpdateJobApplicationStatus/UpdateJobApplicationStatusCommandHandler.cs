using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Commands.UpdateJobApplicationStatus;

public class UpdateJobApplicationStatusCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateJobApplicationStatusCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(
        UpdateJobApplicationStatusCommand request,
        CancellationToken cancellationToken)
    {
        var application = await context.JobApplications
            .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId, cancellationToken);

        if (application is null)
            throw new KeyNotFoundException("Prijava nije pronađena.");

        application.Status = request.Status;
        await context.SaveChangesAsync(cancellationToken);

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
