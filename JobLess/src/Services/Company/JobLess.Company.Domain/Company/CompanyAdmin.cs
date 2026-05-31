using JobLess.Shared.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Domain.Company 
{
    public class CompanyAdmin : BaseEntity //mislim da nam ovo ne treba, ali neka za sad ostane  
    {
        public int CompanyId { get; set; } 
        public int UserId { get; set; }
        public required string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
