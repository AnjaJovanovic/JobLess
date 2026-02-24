using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JobLess.Oglasi.Application.Interfaces;
//using MATFInfostud.Oglasi.Domain.En;
using System.Threading.Tasks;
using JobLess.Oglasi.Domain.Entities;

namespace JobLess.Oglasi.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Oglas> Oglasi { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
