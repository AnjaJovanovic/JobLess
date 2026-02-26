using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Advertisement.Application.Commands.Activate
{
    public class ActivateAdvertisementCommand : IRequest<bool>
    {
        public required int Id { get; set; }
    }
}
