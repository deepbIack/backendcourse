using System.Security.Claims;
using CourseApi.DTOs.Courses;
using CourseApi.Services.Interfaces;

namespace CourseApi.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/courses").WithTags("Courses");

        // GET /api/courses
        group.MapGet("/", async (ICourseService svc) =>
            Results.Ok(await svc.GetAllAsync()))
        .WithName("GetCourses")
        .WithSummary("Все курсы")
        .AllowAnonymous();

        // GET /api/courses/my  — мои курсы
        group.MapGet("/my", async (ClaimsPrincipal p, ICourseService svc) =>
        {
            var uid = GetUserId(p);
            if (uid is null) return Results.Unauthorized();
            return Results.Ok(await svc.GetMyCourseAsync(uid.Value));
        })
        .WithName("GetMyCourses")
        .WithSummary("Курсы созданные мной")
        .RequireAuthorization();

        // GET /api/courses/{id}
        group.MapGet("/{id:int}", async (int id, ClaimsPrincipal p, ICourseService svc) =>
        {
            var course = await svc.GetByIdAsync(id, GetUserId(p));
            return course is null ? Results.NotFound() : Results.Ok(course);
        })
        .WithName("GetCourseById")
        .WithSummary("Курс с уроками")
        .AllowAnonymous();

        // POST /api/courses
        group.MapPost("/", async (CreateCourseRequest req, ClaimsPrincipal p, ICourseService svc) =>
        {
            var uid = GetUserId(p);
            if (uid is null) return Results.Unauthorized();
            var course = await svc.CreateAsync(req, uid.Value);
            return Results.Created($"/api/courses/{course.Id}", course);
        })
        .WithName("CreateCourse")
        .WithSummary("Создать курс")
        .RequireAuthorization();

        // PUT /api/courses/{id}
        group.MapPut("/{id:int}", async (int id, UpdateCourseRequest req, ClaimsPrincipal p, ICourseService svc) =>
        {
            var uid = GetUserId(p);
            if (uid is null) return Results.Unauthorized();
            var result = await svc.UpdateAsync(id, req, uid.Value);
            return result is null ? Results.Forbid() : Results.Ok(result);
        })
        .WithName("UpdateCourse")
        .WithSummary("Обновить курс")
        .RequireAuthorization();

        // DELETE /api/courses/{id}
        group.MapDelete("/{id:int}", async (int id, ClaimsPrincipal p, ICourseService svc) =>
        {
            var uid = GetUserId(p);
            if (uid is null) return Results.Unauthorized();
            var ok = await svc.DeleteAsync(id, uid.Value);
            return ok ? Results.NoContent() : Results.Forbid();
        })
        .WithName("DeleteCourse")
        .WithSummary("Удалить курс")
        .RequireAuthorization();

        // POST /api/courses/{id}/lessons
        group.MapPost("/{id:int}/lessons", async (int id, CreateLessonRequest req, ClaimsPrincipal p, ICourseService svc) =>
        {
            var uid = GetUserId(p);
            if (uid is null) return Results.Unauthorized();
            var lesson = await svc.AddLessonAsync(id, req, uid.Value);
            return lesson is null ? Results.Forbid() : Results.Created($"/api/courses/{id}/lessons/{lesson.Id}", lesson);
        })
        .WithName("AddLesson")
        .WithSummary("Добавить урок")
        .RequireAuthorization();
    }

    private static int? GetUserId(ClaimsPrincipal p)
    {
        var c = p.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(c, out var id) ? id : null;
    }
}
