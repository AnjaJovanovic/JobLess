using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Oglasi.Application.Queries.VratiSveOglase
{
    public class VratiSveOglaseQuery : IRequest<VratiSveOglaseResult>
    {
        public int BrojStranice { get; set; }
        public int VelicinaStranice { get; set; }
        
    }
}
