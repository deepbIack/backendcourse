using CourseApi.Entities;

namespace CourseApi.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();
    Task<IEnumerable<Course>> GetByUserIdAsync(int userId);
    Task<Course?> GetByIdWithLessonsAsync(int id);
    Task<Course?> GetByIdAsync(int id);
    Task<Course> CreateAsync(Course course);
    Task<Course> UpdateAsync(Course course);
    Task DeleteAsync(int id);
}
