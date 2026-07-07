using ClientEntity = JobLess.Client.Domain.Entities.Client;
using JobLess.Client.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<ClientEntity> Clients { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
