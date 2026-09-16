using ToMainApi.Models.Dtos.Pagination;

namespace ToMainApi.Models.Dtos.Payments
{
    public class TransactionFilterDto : PaginationDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
    }
}
