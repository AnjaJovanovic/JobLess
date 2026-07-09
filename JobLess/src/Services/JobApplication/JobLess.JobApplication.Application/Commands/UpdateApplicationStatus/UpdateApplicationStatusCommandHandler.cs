using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Models;
using JobLess.JobApplication.Domain.Enums;
using JobLess.JobApplication.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandHandler(IJobApplicationDbContext context)
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

        return new JobApplicationDto(
            application.Id,
            application.AdvertisementId,
            application.CandidateId,
            application.CandidateEmail,
            application.CandidateFirstName,
            application.CandidateLastName,
            application.CompanyId,
            application.CompanyEmail,
            (int)application.Status,
            application.CreatedAt,
            application.UpdatedAt);
    }
}
