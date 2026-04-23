using CourseApi.DTOs.Progress;
using CourseApi.Repositories.Interfaces;
using CourseApi.Services.Interfaces;

namespace CourseApi.Services;

public class ProgressService : IProgressService
{
    private readonly IProgressRepository _progress;
    private readonly ICourseRepository _courses;

    public ProgressService(IProgressRepository progress, ICourseRepository courses)
    {
        _progress = progress;
        _courses = courses;
    }

    public async Task<IEnumerable<UserProgressDto>> GetUserProgressAsync(int userId)
    {
        var progresses = await _progress.GetByUserIdAsync(userId);

        // Group by course
        var grouped = progresses
            .Where(p => p.IsCompleted)
            .GroupBy(p => p.Lesson.CourseId);

        var result = new List<UserProgressDto>();
        foreach (var group in grouped)
        {
            var course = await _courses.GetByIdWithLessonsAsync(group.Key);
            if (course is null) continue;

            result.Add(new UserProgressDto(
                course.Id,
                course.Title,
                group.Count(),
                course.Lessons.Count));
        }
        return result;
    }
}
