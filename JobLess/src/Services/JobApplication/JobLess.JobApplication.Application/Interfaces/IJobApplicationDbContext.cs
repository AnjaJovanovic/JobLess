using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Application.Interfaces;

public interface IJobApplicationDbContext
{
    DbSet<JobApplicationEntity> JobApplications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
