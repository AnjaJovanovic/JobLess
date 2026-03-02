using JobLess.Advertisement.Application.Models;

namespace JobLess.Advertisement.Application.Queries.Search
{
    public class SearchAdvertisementResult
    {
        public List<AdvertisementModel> Advertisements { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}