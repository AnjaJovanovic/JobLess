using JobLess.JobApplication.Application.Interfaces;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Infrastructure.Persistence;

public class JobApplicationDbContext : DbContext, IJobApplicationDbContext
{
    public JobApplicationDbContext(DbContextOptions<JobApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<JobApplicationEntity> JobApplications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobApplicationEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AdvertisementId, x.CandidateUserId }).IsUnique();
            entity.Property(x => x.CandidateUserId).HasMaxLength(100).IsRequired();
            entity.Property(x => x.CandidateEmail).HasMaxLength(256).IsRequired();
            entity.Property(x => x.CompanyEmail).HasMaxLength(256).IsRequired();
        });
    }
}
