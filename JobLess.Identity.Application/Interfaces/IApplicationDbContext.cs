using JobLess.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Identity.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Client> Clients { get; set; }
    // Asinhrana metoda za čuvanje izmena
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}