using Microsoft.EntityFrameworkCore;
using CourseApi.Data;
using CourseApi.Entities;
using CourseApi.Repositories.Interfaces;

namespace CourseApi.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _db;
    public LessonRepository(AppDbContext db) => _db = db;

    public async Task<Lesson?> GetByIdWithTestAsync(int id) =>
        await _db.Lessons
            .Include(l => l.Test)
                .ThenInclude(t => t!.Questions)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Lesson?> GetByIdAndCourseAsync(int lessonId, int courseId) =>
        await _db.Lessons
            .Include(l => l.Test)
                .ThenInclude(t => t!.Questions)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(l => l.Id == lessonId && l.CourseId == courseId);

    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        _db.Lessons.Add(lesson);
        await _db.SaveChangesAsync();
        return lesson;
    }

    public async Task<Lesson> UpdateAsync(Lesson lesson)
    {
        _db.Lessons.Update(lesson);
        await _db.SaveChangesAsync();
        return lesson;
    }

    public async Task DeleteAsync(int id)
    {
        var lesson = await _db.Lessons.FindAsync(id);
        if (lesson is not null)
        {
            _db.Lessons.Remove(lesson);
            await _db.SaveChangesAsync();
        }
    }
}
