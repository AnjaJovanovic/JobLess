using Microsoft.EntityFrameworkCore; // DbSet dolazi odavde
using JobLess.Advertisement.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JobLess.Advertisement.Domain.Entities;

namespace JobLess.Advertisement.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<JobAdvertisement> JobAdvertisements { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
