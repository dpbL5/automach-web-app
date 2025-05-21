using Microsoft.AspNetCore.Mvc;
using automach_backend.Dto.Auth;
using automach_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace automach_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var result = await _authRepository.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            var result = await _authRepository.RegisterAsync(registerDto);

            if (result == null)
            {
                return BadRequest("Username or email already exists");
            }

            return CreatedAtAction(nameof(Register), result);
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // JWT tokens are stateless, so no server-side action is needed for logout
            // The client should discard the token
            
            // Return a successful response
            return Ok(new { message = "Successfully logged out" });
        }
    }
} 