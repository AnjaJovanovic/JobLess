using JobLess.Advertisement.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobless.Advertisement.Application.Queries.GetAllForCompany
{
    public class GetAllForCompanyResult
    {
        public List<AdvertisementModel>? Advertisements { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
