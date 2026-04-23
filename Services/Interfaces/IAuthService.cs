using CourseApi.DTOs.Auth;

namespace CourseApi.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserDto> GetMeAsync(int userId);
}
