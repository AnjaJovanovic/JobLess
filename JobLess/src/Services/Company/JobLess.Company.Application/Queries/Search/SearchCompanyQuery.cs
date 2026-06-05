using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Queries.Search
{
    public class SearchCompanyQuery : IRequest<SearchCompanyResult>
    {
        public string? Name { get; init; }
        public string? Industry { get; init; }
        public string? Location { get; init; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
