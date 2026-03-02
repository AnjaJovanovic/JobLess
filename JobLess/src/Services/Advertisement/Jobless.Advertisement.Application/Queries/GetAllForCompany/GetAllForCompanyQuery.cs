using JobLess.Advertisement.Application.Queries.GetAll;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobless.Advertisement.Application.Queries.GetAllForCompany
{
    public class GetAllForCompanyQuery : IRequest<GetAllForCompanyResult>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public required int CompanyId { get; set; }

    }
}
