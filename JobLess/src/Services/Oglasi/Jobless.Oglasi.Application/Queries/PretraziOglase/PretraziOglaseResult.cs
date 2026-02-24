using MATFInfostud.Oglasi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATFInfostud.Oglasi.Application.Queries.PretraziOglase
{
    public class PretraziOglaseResult
    {
        public List<OglasModel> Oglasi { get; set; } = new();
        public int BrojStranice { get; set; }
        public int VelicinaStranice { get; set; }
        public int UkupnoStranica { get; set; }
        public int UkupanBroj { get; set; }
    }
}
