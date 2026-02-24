using JobLess.Oglasi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Oglasi.Application.Queries.VratiSveOglase
{
    public class VratiSveOglaseResult
    {
        public List<OglasModel>? Oglasi { get; set; }
        public int BrojStranice { get; set; }
        public int VelicinaStranice { get; set; }
        public int UkupnoStranica { get; set; }
        public int UkupanBroj { get; set; }
    }
}
