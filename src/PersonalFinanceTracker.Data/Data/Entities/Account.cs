using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Data.Data.Entities;

#pragma warning disable CS8618

public class Account : BaseEntity
{
    [MaxLength(36)] public string UserId { get; set; }

    [MaxLength(100)] public string Name { get; set; }

    public decimal Balance { get; set; }

    [MaxLength(50)] public string AccountType { get; set; }
}