using CourseApi.DTOs.Courses;
using CourseApi.Entities;
using CourseApi.Repositories.Interfaces;
using CourseApi.Services.Interfaces;

namespace CourseApi.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courses;
    private readonly ILessonRepository _lessons;
    private readonly IProgressRepository _progress;

    public CourseService(ICourseRepository courses, ILessonRepository lessons, IProgressRepository progress)
    {
        _courses = courses;
        _lessons = lessons;
        _progress = progress;
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        var courses = await _courses.GetAllAsync();
        return courses.Select(ToDto);
    }

    public async Task<IEnumerable<CourseDto>> GetMyCourseAsync(int userId)
    {
        var courses = await _courses.GetByUserIdAsync(userId);
        return courses.Select(ToDto);
    }

    public async Task<CourseDetailDto?> GetByIdAsync(int id, int? userId)
    {
        var course = await _courses.GetByIdWithLessonsAsync(id);
        if (course is null) return null;

        HashSet<int> completedLessons = new();
        if (userId.HasValue)
        {
            var progresses = await _progress.GetByUserIdAsync(userId.Value);
            completedLessons = progresses
                .Where(p => p.IsCompleted && p.Lesson.CourseId == id)
                .Select(p => p.LessonId)
                .ToHashSet();
        }

        var lessonDtos = course.Lessons
            .OrderBy(l => l.Order)
            .Select(l => new LessonSummaryDto(l.Id, l.Title, l.Description, l.Order, completedLessons.Contains(l.Id)))
            .ToList();

        return new CourseDetailDto(course.Id, course.Title, course.Description, course.CoverImage, course.CreatedByUserId, lessonDtos);
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request, int userId)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            CoverImage = request.CoverImage,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _courses.CreateAsync(course);
        return ToDto(course);
    }

    public async Task<CourseDto?> UpdateAsync(int id, UpdateCourseRequest request, int userId)
    {
        var course = await _courses.GetByIdAsync(id);
        if (course is null || course.CreatedByUserId != userId) return null;

        course.Title = request.Title;
        course.Description = request.Description;
        course.CoverImage = request.CoverImage;
        await _courses.UpdateAsync(course);
        return ToDto(course);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var course = await _courses.GetByIdAsync(id);
        if (course is null || course.CreatedByUserId != userId) return false;

        await _courses.DeleteAsync(id);
        return true;
    }

    public async Task<LessonSummaryDto?> AddLessonAsync(int courseId, CreateLessonRequest request, int userId)
    {
        var course = await _courses.GetByIdAsync(courseId);
        if (course is null || course.CreatedByUserId != userId) return null;

        var lesson = new Lesson
        {
            Title = request.Title,
            Description = request.Description,
            Content = request.Content,
            Order = request.Order,
            CourseId = courseId,
            CreatedAt = DateTime.UtcNow
        };
        await _lessons.CreateAsync(lesson);
        return new LessonSummaryDto(lesson.Id, lesson.Title, lesson.Description, lesson.Order, false);
    }

    public async Task<LessonSummaryDto?> UpdateLessonAsync(int lessonId, UpdateLessonRequest request, int userId)
    {
        var lesson = await _lessons.GetByIdWithTestAsync(lessonId);
        if (lesson is null) return null;

        var course = await _courses.GetByIdAsync(lesson.CourseId);
        if (course is null || course.CreatedByUserId != userId) return null;

        lesson.Title = request.Title;
        lesson.Description = request.Description;
        lesson.Content = request.Content;
        lesson.Order = request.Order;
        await _lessons.UpdateAsync(lesson);
        return new LessonSummaryDto(lesson.Id, lesson.Title, lesson.Description, lesson.Order, false);
    }

    public async Task<bool> DeleteLessonAsync(int lessonId, int userId)
    {
        var lesson = await _lessons.GetByIdWithTestAsync(lessonId);
        if (lesson is null) return false;

        var course = await _courses.GetByIdAsync(lesson.CourseId);
        if (course is null || course.CreatedByUserId != userId) return false;

        await _lessons.DeleteAsync(lessonId);
        return true;
    }

    private static CourseDto ToDto(Course c) =>
        new(c.Id, c.Title, c.Description, c.CoverImage, c.CreatedByUserId);
}
