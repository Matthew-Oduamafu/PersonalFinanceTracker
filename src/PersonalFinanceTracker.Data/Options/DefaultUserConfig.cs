using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace PersonalFinanceTracker.Data.Options;

public class DefaultUserConfig
{
    [Required(AllowEmptyStrings = false)] public string Email { get; set; }

    [Required(AllowEmptyStrings = false)] public string Password { get; set; }

    [Required(AllowEmptyStrings = false)] public string PhoneNumber { get; set; }

    [Required(AllowEmptyStrings = false)] public string FirstName { get; set; }

    [Required(AllowEmptyStrings = false)] public string LastName { get; set; }
}