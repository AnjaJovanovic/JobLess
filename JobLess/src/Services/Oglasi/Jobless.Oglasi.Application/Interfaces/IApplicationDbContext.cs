using Microsoft.EntityFrameworkCore; // DbSet dolazi odavde
using MATFInfostud.Oglasi.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MATFInfostud.Oglasi.Domain.Entities;

namespace MATFInfostud.Oglasi.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Oglas> Oglasi { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
