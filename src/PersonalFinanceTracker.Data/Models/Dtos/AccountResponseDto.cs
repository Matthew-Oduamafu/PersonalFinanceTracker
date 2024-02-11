namespace PersonalFinanceTracker.Data.Models.Dtos;

public class AccountResponseDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public string AccountType { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<Link> Links { get; set; } = new();
}