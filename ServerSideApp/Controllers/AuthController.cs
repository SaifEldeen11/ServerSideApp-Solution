using Application.Dtos.Auth;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ServerSideApp.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("auth")]
    public class AuthController(IServiceManger _serviceManager) : ControllerBase
    {
        // =============================================
        // POST api/auth/register
        // =============================================
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _serviceManager.AuthService.RegisterAsync(registerDto);
            return Ok(response);
        }

        // =============================================
        // POST api/auth/login
        // =============================================
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _serviceManager.AuthService.LoginAsync(loginDto);
            return Ok(response);
        }

        // =============================================
        // POST api/auth/change-password
        // =============================================
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _serviceManager.AuthService.ChangePasswordAsync(userId!, changePasswordDto);
            return Ok(new { message = "Password changed successfully" });
        }

        // =============================================
        // POST api/auth/refresh-token
        // =============================================
        [HttpPost("refresh-token")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _serviceManager.AuthService.RefreshTokenAsync(userId!);
            return Ok(response);
        }

        // =============================================
        // GET api/auth/me
        // =============================================
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var fullName = User.FindFirstValue("fullName");
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var coachId = User.FindFirstValue("coachId");
            var swimmerId = User.FindFirstValue("swimmerId");

            return Ok(new
            {
                userId,
                email,
                fullName,
                roles,
                coachId = coachId != null ? int.Parse(coachId) : (int?)null,
                swimmerId = swimmerId != null ? int.Parse(swimmerId) : (int?)null
            });
        }

        // =============================================
        // GET api/auth/check-email/{email}
        // =============================================
        [HttpGet("check-email/{email}")]
        [Authorize(Roles = "Admin,SeniorCoach")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            var exists = await _serviceManager.AuthService.UserExistsAsync(email);
            return Ok(new { exists });
        }
    }
}