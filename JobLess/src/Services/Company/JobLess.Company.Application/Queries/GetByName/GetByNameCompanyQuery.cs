using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Queries.GetByName
{
    public class GetByNameCompanyQuery : IRequest<GetByNameCompanyResult>
    {
        public required string Name { get; set; }
    }
}
