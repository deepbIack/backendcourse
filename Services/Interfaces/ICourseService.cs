using CourseApi.DTOs.Courses;

namespace CourseApi.Services.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task<IEnumerable<CourseDto>> GetMyCourseAsync(int userId);
    Task<CourseDetailDto?> GetByIdAsync(int id, int? userId);
    Task<CourseDto> CreateAsync(CreateCourseRequest request, int userId);
    Task<CourseDto?> UpdateAsync(int id, UpdateCourseRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);

    // Lessons
    Task<LessonSummaryDto?> AddLessonAsync(int courseId, CreateLessonRequest request, int userId);
    Task<LessonSummaryDto?> UpdateLessonAsync(int lessonId, UpdateLessonRequest request, int userId);
    Task<bool> DeleteLessonAsync(int lessonId, int userId);
}
