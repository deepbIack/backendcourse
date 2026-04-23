using CourseApi.DTOs.Progress;

namespace CourseApi.Services.Interfaces;

public interface IProgressService
{
    Task<IEnumerable<UserProgressDto>> GetUserProgressAsync(int userId);
}
