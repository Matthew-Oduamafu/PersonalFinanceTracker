using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Data.Data.Entities;

#pragma warning disable CS8618

public class Transaction : BaseEntity
{
    [MaxLength(36)]
    public string AccountId { get; set; }
    [MaxLength(300)]
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    [MaxLength(50)]
    public string TransactionType { get; set; }
}