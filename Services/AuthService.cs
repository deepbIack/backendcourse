using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CourseApi.DTOs.Auth;
using CourseApi.Entities;
using CourseApi.Repositories.Interfaces;
using CourseApi.Services.Interfaces;

namespace CourseApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _users.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _users.CreateAsync(user);
        var token = GenerateJwt(user);
        return new AuthResponse(token, ToDto(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Неверный email или пароль");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Неверный email или пароль");

        return new AuthResponse(GenerateJwt(user), ToDto(user));
    }

    public async Task<UserDto> GetMeAsync(int userId)
    {
        var user = await _users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Пользователь не найден");
        return ToDto(user);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto ToDto(User u) => new(u.Id, u.Name, u.Email);
}
