using System.Security.Claims;
using CourseApi.DTOs.Auth;
using CourseApi.Services.Interfaces;

namespace CourseApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // POST /api/auth/register
        group.MapPost("/register", async (RegisterRequest req, IAuthService svc) =>
        {
            try
            {
                var result = await svc.RegisterAsync(req);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("Register")
        .WithSummary("Регистрация нового пользователя")
        .AllowAnonymous();

        // POST /api/auth/login
        group.MapPost("/login", async (LoginRequest req, IAuthService svc) =>
        {
            try
            {
                var result = await svc.LoginAsync(req);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Unauthorized();
            }
        })
        .WithName("Login")
        .WithSummary("Вход пользователя")
        .AllowAnonymous();

        // GET /api/auth/me
        group.MapGet("/me", async (ClaimsPrincipal principal, IAuthService svc) =>
        {
            var userId = GetUserId(principal);
            if (userId is null) return Results.Unauthorized();

            try
            {
                var user = await svc.GetMeAsync(userId.Value);
                return Results.Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithName("GetMe")
        .WithSummary("Получить текущего пользователя")
        .RequireAuthorization();
    }

    private static int? GetUserId(ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}
