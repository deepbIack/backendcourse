using CourseApi.DTOs.Lessons;
using CourseApi.Entities;
using CourseApi.Repositories.Interfaces;
using CourseApi.Services.Interfaces;

namespace CourseApi.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessons;
    private readonly IProgressRepository _progress;

    public LessonService(ILessonRepository lessons, IProgressRepository progress)
    {
        _lessons = lessons;
        _progress = progress;
    }

    public async Task<LessonDetailDto?> GetLessonAsync(int courseId, int lessonId, int userId)
    {
        var lesson = await _lessons.GetByIdAndCourseAsync(lessonId, courseId);
        if (lesson is null) return null;

        var prog = await _progress.GetAsync(userId, lessonId);
        bool isCompleted = prog?.IsCompleted ?? false;

        TestDto? testDto = null;
        if (lesson.Test is not null)
        {
            testDto = new TestDto(
                lesson.Test.Id,
                lesson.Test.Title,
                lesson.Test.Questions.Select(q => new QuestionDto(
                    q.Id,
                    q.Text,
                    q.CorrectAnswerId,
                    q.Options.Select(o => new AnswerDto(o.Id, o.Text)).ToList()
                )).ToList());
        }

        return new LessonDetailDto(
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.Content,
            lesson.CourseId,
            isCompleted,
            testDto);
    }

    public async Task CompleteLessonAsync(int lessonId, int userId, double score)
    {
        var progress = new UserProgress
        {
            UserId = userId,
            LessonId = lessonId,
            Score = score,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow
        };
        await _progress.UpsertAsync(progress);
    }
}
