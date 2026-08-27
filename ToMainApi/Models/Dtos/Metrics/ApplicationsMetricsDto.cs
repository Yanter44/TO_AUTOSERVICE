namespace ToMainApi.Models.Dtos.Metrics
{
    public class ApplicationsMetricsDto
    {
        public int TotalApplicationsCount { get; set; }
        public int TotalApplicationsInModerationCount { get; set; }
        public int TotalApplicationsApprovedCount { get; set; }
        public int TotalApplicationsTodayCount { get; set; }
    }
}
