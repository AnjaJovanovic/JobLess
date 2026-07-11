using JobLess.Contracts.Events;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Models;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace JobLess.JobApplication.Application.Commands.ApplyForJob;

public class ApplyForJobCommandHandler(
    IJobApplicationDbContext context,
    IClientProfileLookupService clientProfileLookupService,
    ICompanyLookupService companyLookupService,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<ApplyForJobCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(ApplyForJobCommand request, CancellationToken cancellationToken)
    {
        var clientProfile = await clientProfileLookupService.GetByEmailAsync(request.CandidateEmail, cancellationToken);
        if (clientProfile is null)
        {
            throw new InvalidOperationException("Profil kandidata nije pronađen.");
        }

        var company = await companyLookupService.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company is null)
        {
            throw new InvalidOperationException("Kompanija nije pronađena.");
        }

        var exists = await context.JobApplications
            .AsNoTracking()
            .AnyAsync(x => x.AdvertisementId == request.AdvertisementId && x.CandidateId == clientProfile.ClientId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Već ste prijavljeni na ovaj oglas.");
        }

        var application = JobApplicationEntity.Create(
            request.AdvertisementId,
            clientProfile.ClientId,
            clientProfile.Email,
            clientProfile.FirstName,
            clientProfile.LastName,
            company.CompanyId,
            company.Email);

        context.JobApplications.Add(application);
        await context.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new JobAppliedMessage(
            application.Id,
            application.AdvertisementId,
            application.CandidateId,
            application.CandidateEmail,
            application.CandidateFirstName,
            application.CandidateLastName,
            application.CompanyId,
            application.CompanyEmail), cancellationToken);

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
