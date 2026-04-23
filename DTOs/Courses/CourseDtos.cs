namespace CourseApi.DTOs.Courses;

// ── Existing DTOs (keep these) ──────────────────────────────────────
public record CourseDto(
    int Id,
    string Title,
    string Description,
    string? CoverImage,
    int? CreatedByUserId
);

public record CourseDetailDto(
    int Id,
    string Title,
    string Description,
    string? CoverImage,
    int? CreatedByUserId,
    List<LessonSummaryDto> Lessons
);

public record LessonSummaryDto(
    int Id,
    string Title,
    string Description,
    int Order,
    bool IsCompleted
);

// ── New: Create / Update ────────────────────────────────────────────
public record CreateCourseRequest(string Title, string Description, string? CoverImage);

public record UpdateCourseRequest(string Title, string Description, string? CoverImage);

public record CreateLessonRequest(string Title, string Description, string Content, int Order);

public record UpdateLessonRequest(string Title, string Description, string Content, int Order);
