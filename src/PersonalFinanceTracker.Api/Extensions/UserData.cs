namespace PersonalFinanceTracker.Api.Extensions;

public static class UserData
{
    public static (string email, string userId) GetUserEmail(HttpContext context)
    {
        var userEmail = context.User.Claims.FirstOrDefault(claim => claim.Type.EndsWith("emailaddress"))?.Value ??
                        string.Empty;
        var userId = context.User.Claims.FirstOrDefault(claim => claim.Type.EndsWith("nameidentifier"))?.Value ??
                     string.Empty;
        return (userEmail, userId);
    }
}