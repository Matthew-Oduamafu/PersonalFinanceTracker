using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Options;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class AuthManager(
    ILogger<AuthManager> logger,
    IOptionsMonitor<JwtConfig> jwtConfigOpt,
    UserManager<AppUser> userManager)
    : IAuthManager
{
    private const string RefreshToken = "RefreshToken";
    private const string LoginProvider = "PersonalFinanceTracker";
    private readonly JwtConfig _jwtConfig = jwtConfigOpt.CurrentValue;
    private AppUser _user;

    public async Task<IGenericApiResponse<LoginOrRegisterResponseDto>> RegisterUserAsync(AppUserDto user)
    {
        try
        {
            var existingUser = await userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToBadRequestApiResponse(
                    "User already exists");

            existingUser = await userManager.FindByNameAsync(user.UserName);
            if (existingUser != null)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToBadRequestApiResponse(
                    "User already exists");

            _user = user.Adapt<AppUser>();

            var result = await userManager.CreateAsync(_user, user.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new ErrorResponse(e.Code, e.Description));
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToFailedDependenciesApiResponse(
                    errors: errors);
            }

            await userManager.AddToRoleAsync(_user, "User");

            var response = new LoginOrRegisterResponseDto
            {
                Token = await GenerateToken(),
                RefreshToken = await CreateRefreshTokenAsync()
            };

            return response.ToCreatedApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<LoginOrRegisterResponseDto>> LoginAsync(LoginUserDto user)
    {
        try
        {
            _user = await userManager.FindByEmailAsync(user.Username);
            if (_user == null)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToUnAuthorizedApiResponse(
                    "Email is not registered");

            var isCorrect = await userManager.CheckPasswordAsync(_user, user.Password);

            if (!isCorrect)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToUnAuthorizedApiResponse(
                    "Invalid password");

            var token = await GenerateToken();
            var refreshToken = await CreateRefreshTokenAsync();

            var response = new LoginOrRegisterResponseDto
            {
                Token = token,
                RefreshToken = refreshToken
            };

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<string> CreateRefreshTokenAsync()
    {
        await userManager.RemoveAuthenticationTokenAsync(_user ?? throw new InvalidOperationException(), LoginProvider,
            RefreshToken);
        var newRefreshToken = await userManager.GenerateUserTokenAsync(_user, LoginProvider, RefreshToken);

        var result =
            await userManager.SetAuthenticationTokenAsync(_user, LoginProvider, RefreshToken, newRefreshToken);

        return newRefreshToken;
    }

    public async Task<IGenericApiResponse<RefreshTokenResponseDto>> VerifyRefreshTokenAsync(
        LoginOrRegisterResponseDto request)
    {
        try
        {
            var jwtTokenContent = new JwtSecurityTokenHandler().ReadJwtToken(request.Token);

            var userEmail =
                jwtTokenContent.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email)?.Value ??
                string.Empty;

            _user = await userManager.FindByEmailAsync(userEmail);

            if (_user == null)
                return GenericApiResponse<RefreshTokenResponseDto>.Default.ToUnAuthorizedApiResponse(
                    "Email is not registered");

            var isCorrect =
                await userManager.VerifyUserTokenAsync(_user, LoginProvider, RefreshToken, request.RefreshToken);

            if (!isCorrect)
            {
                await userManager.UpdateSecurityStampAsync(_user);

                return GenericApiResponse<RefreshTokenResponseDto>.Default.ToUnAuthorizedApiResponse(
                    "Invalid refresh token");
            }

            var token = await GenerateToken();
            var refreshToken = await CreateRefreshTokenAsync();

            var response = new RefreshTokenResponseDto
            {
                Token = token,
                RefreshToken = refreshToken
            };

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<RefreshTokenResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }


    /***
     * Generate token
     */
    private async Task<string> GenerateToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
        var secret = new SymmetricSecurityKey(key);

        var roles = await userManager.GetRolesAsync(_user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        var userClaims = await userManager.GetClaimsAsync(_user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email!),
                new Claim(ClaimTypes.Name, _user.UserName!),
                new Claim("UserEmail", _user.Email!),
                new Claim("FullName", $"{_user.FirstName} {_user.LastName}")
            }.Union(roleClaims).Union(userClaims)),

            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiration),
            SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    /***
    private async Task<string> GenerateTokenV2(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
        var secret = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature);

        // var roles = Get roles from user or DB;
        // create claims out of roles
        // get user claims for db or user if any


        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, "User"),
        };

        var token = new JwtSecurityToken(
            _jwtConfig.Issuer,
            _jwtConfig.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiration),
            signingCredentials: signingCredentials
        );

        return tokenHandler.WriteToken(token);
    }
    */
}