using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Options;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class AuthManager : IAuthManager
{
    private readonly ILogger<AuthManager> _logger;
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<AppUser> _userManager;

    public AuthManager(ILogger<AuthManager> logger,
        IOptionsMonitor<JwtConfig> jwtConfigOpt,
        UserManager<AppUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
        _jwtConfig = jwtConfigOpt.CurrentValue;
    }

    public async Task<IGenericApiResponse<LoginOrRegisterResponseDto>> RegisterUserAsync(AppUserDto user)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToBadRequestApiResponse(
                    "User already exists");

            existingUser = await _userManager.FindByNameAsync(user.UserName);
            if (existingUser != null)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToBadRequestApiResponse(
                    "User already exists");

            var newUser = user.Adapt<AppUser>();

            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new ErrorResponse(e.Code, e.Description));
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToFailedDependenciesApiResponse(
                    errors: errors);
            }

            await _userManager.AddToRoleAsync(newUser, "User");

            var response = new LoginOrRegisterResponseDto
            {
                Token = await GenerateToken(newUser)
            };

            return response.ToCreatedApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<LoginOrRegisterResponseDto>> LoginAsync(LoginUserDto user)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Username);
            if (existingUser == null)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToBadRequestApiResponse(
                    "Email is not registered");

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

            if (!isCorrect)
                return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToBadRequestApiResponse(
                    "Invalid password");

            var response = new LoginOrRegisterResponseDto
            {
                Token = await GenerateToken(existingUser)
            };

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return GenericApiResponse<LoginOrRegisterResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    /***
     * Generate token
     */
    private async Task<string> GenerateToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
        var secret = new SymmetricSecurityKey(key);

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        var userClaims = await _userManager.GetClaimsAsync(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
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