using System.Text.Json.Serialization;

namespace PersonalFinanceTracker.Data.Models.Dtos;

public class TransactionRequest
{
    public string AccountId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
}

public class CreateTransactionRequestDto : TransactionRequest
{
    [JsonIgnore] public string CreatedBy { get; set; }
}

public class UpdateTransactionRequestDto : TransactionRequest
{
    [JsonIgnore] public string UpdatedBy { get; set; }
}