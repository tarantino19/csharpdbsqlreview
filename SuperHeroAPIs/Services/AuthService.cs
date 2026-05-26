using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SuperHeroAPIs.DTOs.Auth;
using SuperHeroAPIs.Entities;
using SuperHeroAPIs.Repositories;

namespace SuperHeroAPIs.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly string _jwtSecret;

        public AuthService(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _jwtSecret = configuration["JWT_SECRET"]
                ?? throw new InvalidOperationException("JWT_SECRET not configured.");
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _repository.GetByEmailAsync(dto.Email);
            if (existing is not null)
                throw new InvalidOperationException("Account operation cannot be completed, please choose another user or password");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var created = await _repository.CreateAsync(user);
            return MapToDto(created);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return GenerateToken(user);
        }

        public async Task<UserDto> GetCurrentUserAsync(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user is null)
                throw new InvalidOperationException("User not found.");

            return MapToDto(user);
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static UserDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Email = user.Email
        };
    }
}
