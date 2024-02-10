using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface IAuthManager
{
    Task<IGenericApiResponse<LoginOrRegisterResponseDto>> RegisterUserAsync(AppUserDto user);
    Task<IGenericApiResponse<LoginOrRegisterResponseDto>> LoginAsync(LoginUserDto user);
}