using JobLess.Advertisement.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Advertisement.Application.Queries.GetOne
{
    public class GetOneAdvertisementResult
    {
        public AdvertisementModel Advertisement { get; set; } = new();
       
    }
}
