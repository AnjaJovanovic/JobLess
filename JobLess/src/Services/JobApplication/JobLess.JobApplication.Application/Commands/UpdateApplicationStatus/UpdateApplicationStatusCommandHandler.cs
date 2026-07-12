using JobLess.Contracts.Events;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Mappings;
using JobLess.JobApplication.Application.Models;
using JobLess.JobApplication.Domain.Enums;
using JobLess.JobApplication.Domain.Exceptions;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandHandler(IJobApplicationDbContext context, IPublishEndpoint publishEndpoint)
    : IRequestHandler<UpdateApplicationStatusCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(
        UpdateApplicationStatusCommand request,
        CancellationToken cancellationToken)
    {
        var application = await context.JobApplications
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationId, cancellationToken);

        if (application is null)
        {
            throw new KeyNotFoundException("Prijava nije pronađena.");
        }

        var companyEmail = request.CompanyEmail.Trim().ToLowerInvariant();
        if (!string.Equals(application.CompanyEmail, companyEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Nemate dozvolu da menjate ovu prijavu.");
        }

        try
        {
            if (request.Status == JobApplicationStatus.Accepted)
            {
                application.Accept();
            }
            else
            {
                application.Reject();
            }
        }
        catch (InvalidJobApplicationStatusTransitionException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }

        await context.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new ApplicationStatusChangedMessage(
            application.Id,
            application.AdvertisementId,
            application.CompanyId,
            application.CandidateEmail,
            application.CandidateFirstName,
            application.CandidateLastName,
            application.Status.ToString()), cancellationToken);

        return JobApplicationMapper.ToDto(application);
    }
}
