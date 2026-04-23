using System.Security.Claims;
using CourseApi.Services.Interfaces;

namespace CourseApi.Endpoints;

public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Progress");

        // GET /api/users/me/progress
        group.MapGet("/me/progress", async (ClaimsPrincipal principal, IProgressService svc) =>
        {
            var userId = GetUserId(principal);
            if (userId is null) return Results.Unauthorized();

            var progress = await svc.GetUserProgressAsync(userId.Value);
            return Results.Ok(progress);
        })
        .WithName("GetMyProgress")
        .WithSummary("Получить прогресс текущего пользователя по всем курсам")
        .RequireAuthorization();
    }

    private static int? GetUserId(ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}
