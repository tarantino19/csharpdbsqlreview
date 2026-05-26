using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperHeroAPIs.DTOs.Auth;
using SuperHeroAPIs.Services;

namespace SuperHeroAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto dto)
        {
            try
            {
                await _authService.RegisterAsync(dto);
                return Ok(new { message = "Account created successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);

                Response.Cookies.Append("accessToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });

                return Ok(new { message = "Logged in successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _authService.GetCurrentUserAsync(userId);
            return Ok(user);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            return Ok(new { message = "Logged out successfully." });
        }

    }
}
