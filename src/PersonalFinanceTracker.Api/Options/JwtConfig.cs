using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace PersonalFinanceTracker.Api.Options;

public class JwtConfig
{
    [Required(AllowEmptyStrings = false)] public string Issuer { get; set; }

    [Required(AllowEmptyStrings = false)] public string Audience { get; set; }

    [Required(AllowEmptyStrings = false)] public string Key { get; set; }

    /***
     * In minutes
     */
    [Required] [Range(0, int.MaxValue)] public int AccessTokenExpiration { get; set; }

    [Required] [Range(0, int.MaxValue)] public int RefreshTokenExpiration { get; set; }
}