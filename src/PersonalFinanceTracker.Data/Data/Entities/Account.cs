namespace PersonalFinanceTracker.Data.Data.Entities;

#pragma warning disable CS8618

public class Account : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public string AccountType { get; set; }
}