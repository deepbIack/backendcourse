using CourseApi.DTOs.Lessons;

namespace CourseApi.Services.Interfaces;

public interface ILessonService
{
    Task<LessonDetailDto?> GetLessonAsync(int courseId, int lessonId, int userId);
    Task CompleteLessonAsync(int lessonId, int userId, double score);
}
