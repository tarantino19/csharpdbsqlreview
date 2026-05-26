using SuperHeroAPIs.DTOs.Auth;

namespace SuperHeroAPIs.Services
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<UserDto> GetCurrentUserAsync(int userId);
    }
}
