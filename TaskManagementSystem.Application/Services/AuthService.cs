using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Repositories;
using TaskManagementSystem.Application.Interfaces.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
    {
        // 1. التأكد إن الإيميل مش موجود قبل كدا
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto { Message = "Email is already registered!" };
        }

        // 2. التأكد من صحة الـ Role
        if (!Enum.TryParse(model.Role, true, out UserRole userRole))
        {
            return new AuthResponseDto { Message = "Invalid Role." };
        }

        // 3. إنشاء المستخدم وتشفير الباسورد
        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = userRole
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Message = "User registered successfully."
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto model)
    {
        // 1. البحث عن المستخدم
        var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);

        // 2. التأكد من المستخدم وصحة الباسورد
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            return new AuthResponseDto { Message = "Invalid Email or Password." };
        }

        // 3. إنشاء التوكن
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Token = token,
            Message = "Login successful.",
            Roles = new List<string> { user.Role.ToString() }
        };
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("uid", user.Id.ToString()) // هنستخدمه لاحقاً عشان نعرف الـ ID بتاع اللي عامل Login
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}