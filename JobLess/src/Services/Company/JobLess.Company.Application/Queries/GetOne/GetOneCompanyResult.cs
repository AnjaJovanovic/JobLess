using JobLess.Company.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Queries.GetOne
{
    public class GetOneCompanyResult
    {
        public CompanyModel Company { get; set; } = new();
    }
}
