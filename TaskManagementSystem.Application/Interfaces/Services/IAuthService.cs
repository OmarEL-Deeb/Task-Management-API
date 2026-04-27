using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);
    Task<AuthResponseDto> LoginAsync(LoginDto model);
}