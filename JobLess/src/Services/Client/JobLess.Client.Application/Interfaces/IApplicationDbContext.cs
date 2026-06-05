using ClientEntity = JobLess.Client.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ClientEntity> Clients { get; set; }
    // Asinhrana metoda za čuvanje izmena
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}