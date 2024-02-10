namespace PersonalFinanceTracker.Data.Data.Entities;

#pragma warning disable CS8618

public class Transaction : BaseEntity
{
    public string AccountId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
}