namespace PersonalFinanceTracker.Api.Models.RequestFilters;

public class AccountFilter : BaseFilter
{
    public string? Name { get; set; }
    public string? AccountType { get; set; }
    public string? CreatedBy { get; set; }
    public string? SortDir { get; set; } = "asc";
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}