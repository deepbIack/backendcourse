using Microsoft.EntityFrameworkCore;
using CourseApi.Data;
using CourseApi.Entities;
using CourseApi.Repositories.Interfaces;

namespace CourseApi.Repositories;

public class ProgressRepository : IProgressRepository
{
    private readonly AppDbContext _db;

    public ProgressRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UserProgress?> GetAsync(int userId, int lessonId) =>
        await _db.UserProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

    public async Task<IEnumerable<UserProgress>> GetByUserIdAsync(int userId) =>
        await _db.UserProgresses
            .Include(p => p.Lesson)
                .ThenInclude(l => l.Course)
            .Where(p => p.UserId == userId)
            .ToListAsync();

    public async Task<UserProgress> UpsertAsync(UserProgress progress)
    {
        var existing = await GetAsync(progress.UserId, progress.LessonId);
        if (existing is null)
        {
            _db.UserProgresses.Add(progress);
        }
        else
        {
            existing.Score = progress.Score;
            existing.IsCompleted = progress.IsCompleted;
            existing.CompletedAt = DateTime.UtcNow;
            _db.UserProgresses.Update(existing);
        }
        await _db.SaveChangesAsync();
        return existing ?? progress;
    }
}
