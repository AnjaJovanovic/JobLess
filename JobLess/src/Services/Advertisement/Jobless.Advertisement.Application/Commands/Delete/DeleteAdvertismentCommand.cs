using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Advertisement.Application.Commands.Delete
{
    public class DeleteAdvertismentCommand : IRequest<bool>
    {
        public required int Id { get; set; }
    }
}
