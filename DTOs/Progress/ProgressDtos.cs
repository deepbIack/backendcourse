namespace CourseApi.DTOs.Progress;

public record UserProgressDto(
    int CourseId,
    string CourseTitle,
    int CompletedLessons,
    int TotalLessons
);
