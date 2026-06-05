using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Queries.GetOne
{
    public class GetOneCompanyQuery : IRequest<GetOneCompanyResult>
    {
        public required int Id { get; set; }
    }
}
