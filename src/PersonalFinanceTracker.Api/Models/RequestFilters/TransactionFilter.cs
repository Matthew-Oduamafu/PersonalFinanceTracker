namespace PersonalFinanceTracker.Api.Models.RequestFilters;

public class TransactionFilter : BaseFilter
{
    public string? AccountId { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public string? TransactionType { get; set; }
    public DateTime? FromTransactionDate { get; set; }
    public DateTime? ToTransactionDate { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}