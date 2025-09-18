// Controllers/AuthController.cs
using KnowledgeHub.Api.DTOs;
using KnowledgeHub.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeHub.Api.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result == null ? BadRequest("Registration failed") : Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result == null ? Unauthorized("Invalid credentials") : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole(string userName, string role)
        {
            var result = await _authService.ChangeRoleAsync(userName, role);
            return result ? Ok("Role updated") : BadRequest("Failed to update role");
        }
    }
}
