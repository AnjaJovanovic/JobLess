using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Advertisement.Application.Queries.GetOne
{
    public class GetOneAdvertisementQuery : IRequest<GetOneAdvertisementResult>
    {
        public required int Id { get; set; }

    }
}
