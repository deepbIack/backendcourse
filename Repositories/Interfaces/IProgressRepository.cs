using CourseApi.Entities;

namespace CourseApi.Repositories.Interfaces;

public interface IProgressRepository
{
    Task<UserProgress?> GetAsync(int userId, int lessonId);
    Task<IEnumerable<UserProgress>> GetByUserIdAsync(int userId);
    Task<UserProgress> UpsertAsync(UserProgress progress);
}
