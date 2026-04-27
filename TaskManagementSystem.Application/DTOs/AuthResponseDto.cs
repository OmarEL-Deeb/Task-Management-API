namespace TaskManagementSystem.Application.DTOs;

public class AuthResponseDto
{
    public string Message { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}