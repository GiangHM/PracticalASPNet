using Microsoft.Extensions.Options;

namespace PracticalAPI.LimiterMiddleware.Models
{
    public class RateLimitOptions
    {
        public int Window { get; set; }
        public int QueueLimit { get; set; }
        public int PermitLimit { get; set; }
        public int SegmentsPerWindow { get; set; }
    }
}
