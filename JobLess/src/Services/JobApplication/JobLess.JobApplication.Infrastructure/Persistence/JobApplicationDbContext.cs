using JobLess.JobApplication.Application.Interfaces;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobLess.JobApplication.Infrastructure.Persistence;

public class JobApplicationDbContext : DbContext, IJobApplicationDbContext
{
    public JobApplicationDbContext(DbContextOptions<JobApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<JobApplicationEntity> JobApplications => Set<JobApplicationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JobApplicationEntity>(entity =>
        {
            entity.ToTable("JobApplications");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.Property(x => x.CandidateEmail).HasMaxLength(256).IsRequired();
            entity.Property(x => x.CandidateFirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.CandidateLastName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.CompanyEmail).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.RowVersion).IsRowVersion();

            entity.HasIndex(x => new { x.AdvertisementId, x.CandidateId })
                .IsUnique()
                .HasDatabaseName("IX_JobApplications_AdvertisementId_CandidateId");

            entity.HasIndex(x => x.CandidateId)
                .HasDatabaseName("IX_JobApplications_CandidateId");

            entity.HasIndex(x => x.CompanyId)
                .HasDatabaseName("IX_JobApplications_CompanyId");

            entity.HasIndex(x => new { x.CompanyId, x.AdvertisementId, x.Status })
                .HasDatabaseName("IX_JobApplications_CompanyId_AdvertisementId_Status");
        });
    }
}

public class JobApplicationDbContextFactory : IDesignTimeDbContextFactory<JobApplicationDbContext>
{
    public JobApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<JobApplicationDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=JobLessJobApplicationDb;User Id=sa;Password=JobLess_Pass123!;TrustServerCertificate=True;")
            .Options;

        return new JobApplicationDbContext(options);
    }
}
