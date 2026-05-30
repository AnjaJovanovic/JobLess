using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Advertisement.Application.Queries.GetAll
{
    public class GetAllAdvertisementQuery : IRequest<GetAllAdvertisementResult>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        
    }
}
