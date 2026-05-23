using JobLess.Company.Domain.Company;
using JobLess.Company.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Domain.Entities.Company> Companies { get; set; }
        DbSet<CompanyAdmin> CompanyAdmins { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
