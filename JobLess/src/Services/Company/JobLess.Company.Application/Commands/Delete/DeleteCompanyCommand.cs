using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Commands.Delete
{
    public class DeleteCompanyCommand : IRequest<bool>
    {
        public required int Id { get; set; }
        public string CompanyEmail { get; set; } = string.Empty;

    }
}
