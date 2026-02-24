using Microsoft.EntityFrameworkCore; // DbSet dolazi odavde
using JobLess.Oglasi.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JobLess.Oglasi.Domain.Entities;

namespace JobLess.Oglasi.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Oglas> Oglasi { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
