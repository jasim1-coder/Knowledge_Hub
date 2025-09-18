// Services/Interfaces/IAuthService.cs
using KnowledgeHub.Api.DTOs;

namespace KnowledgeHub.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<bool> ChangeRoleAsync(string userName, string role);
    }
}
