using ClientEntity = JobLess.Client.Domain.Entities.Client;
using JobLess.Client.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ClientEntity> Clients { get; set; }
    DbSet<JobApplication> JobApplications { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}