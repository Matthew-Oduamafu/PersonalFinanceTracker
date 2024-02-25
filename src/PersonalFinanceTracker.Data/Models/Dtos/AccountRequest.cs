using System.Text.Json.Serialization;

namespace PersonalFinanceTracker.Data.Models.Dtos;

public class AccountRequest
{
    [JsonIgnore] public string UserId { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; } = 0;
    public string AccountType { get; set; }
}

public class CreateAccountRequestDto : AccountRequest
{
    [JsonIgnore] public string CreatedBy { get; set; }
}

public class UpdateAccountRequestDto : AccountRequest
{
    [JsonIgnore] public string UpdatedBy { get; set; }
}