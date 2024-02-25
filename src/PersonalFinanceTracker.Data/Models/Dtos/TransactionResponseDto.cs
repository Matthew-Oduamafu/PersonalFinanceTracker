namespace PersonalFinanceTracker.Data.Models.Dtos;

public class TransactionResponseDto
{
    public string Id { get; set; }
    public string AccountId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Link> Links { get; set; }
}