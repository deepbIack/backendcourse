using System.Security.Claims;
using CourseApi.DTOs.Courses;
using CourseApi.DTOs.Lessons;
using CourseApi.Services.Interfaces;

namespace CourseApi.Endpoints;

public static class LessonEndpoints
{
    public static void MapLessonEndpoints(this WebApplication app)
    {
        // GET /api/courses/{courseId}/lessons/{lessonId}
        app.MapGet("/api/courses/{courseId:int}/lessons/{lessonId:int}",
            async (int courseId, int lessonId, ClaimsPrincipal principal, ILessonService svc) =>
            {
                var userId = GetUserId(principal);
                if (userId is null) return Results.Unauthorized();
                var lesson = await svc.GetLessonAsync(courseId, lessonId, userId.Value);
                return lesson is null ? Results.NotFound() : Results.Ok(lesson);
            })
        .WithTags("Lessons")
        .WithName("GetLesson")
        .WithSummary("Урок с тестом")
        .RequireAuthorization();

        // POST /api/lessons/{lessonId}/complete
        app.MapPost("/api/lessons/{lessonId:int}/complete",
            async (int lessonId, CompleteLessonRequest req, ClaimsPrincipal principal, ILessonService svc) =>
            {
                var userId = GetUserId(principal);
                if (userId is null) return Results.Unauthorized();
                await svc.CompleteLessonAsync(lessonId, userId.Value, req.Score);
                return Results.Ok(new { message = "Урок отмечен как пройденный" });
            })
        .WithTags("Lessons")
        .WithName("CompleteLesson")
        .WithSummary("Завершить урок")
        .RequireAuthorization();

        // PUT /api/lessons/{lessonId}
        app.MapPut("/api/lessons/{lessonId:int}",
            async (int lessonId, UpdateLessonRequest req, ClaimsPrincipal principal, ICourseService svc) =>
            {
                var userId = GetUserId(principal);
                if (userId is null) return Results.Unauthorized();
                var result = await svc.UpdateLessonAsync(lessonId, req, userId.Value);
                return result is null ? Results.Forbid() : Results.Ok(result);
            })
        .WithTags("Lessons")
        .WithName("UpdateLesson")
        .WithSummary("Обновить урок")
        .RequireAuthorization();

        // DELETE /api/lessons/{lessonId}
        app.MapDelete("/api/lessons/{lessonId:int}",
            async (int lessonId, ClaimsPrincipal principal, ICourseService svc) =>
            {
                var userId = GetUserId(principal);
                if (userId is null) return Results.Unauthorized();
                var ok = await svc.DeleteLessonAsync(lessonId, userId.Value);
                return ok ? Results.NoContent() : Results.Forbid();
            })
        .WithTags("Lessons")
        .WithName("DeleteLesson")
        .WithSummary("Удалить урок")
        .RequireAuthorization();
    }

    private static int? GetUserId(ClaimsPrincipal p)
    {
        var c = p.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(c, out var id) ? id : null;
    }
}
