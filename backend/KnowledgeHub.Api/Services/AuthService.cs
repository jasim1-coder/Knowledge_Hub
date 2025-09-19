// Services/AuthService.cs
using KnowledgeHub.Api.DTOs;
using KnowledgeHub.Api.Models;
using KnowledgeHub.Api.Services.Interface;
using KnowledgeHub.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace KnowledgeHub.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IJwtService _jwtService;

        public AuthService(UserManager<User> userManager,
                           RoleManager<IdentityRole<Guid>> roleManager,
                           IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email
                
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return null;

            // Default role = "User"
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));

            await _userManager.AddToRoleAsync(user, "User");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user.UserName, roles);

            return new AuthResponseDto
            {
                UserName = user.UserName!,
                Email = user.Email!,
                Token = token,
                UserId = user.Id,
                Roles = roles.ToList()
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user.UserName, roles);

            return new AuthResponseDto
            {
                UserName = user.UserName!,
                Email = user.Email!,
                Token = token,
                UserId = user.Id,
                Roles = roles.ToList()
            };
        }

        public async Task<bool> ChangeRoleAsync(string userName, string role)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return false;

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, role);
            return true;
        }
    }
}
