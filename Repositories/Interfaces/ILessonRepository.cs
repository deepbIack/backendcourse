using CourseApi.Entities;

namespace CourseApi.Repositories.Interfaces;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdWithTestAsync(int id);
    Task<Lesson?> GetByIdAndCourseAsync(int lessonId, int courseId);
    Task<Lesson> CreateAsync(Lesson lesson);
    Task<Lesson> UpdateAsync(Lesson lesson);
    Task DeleteAsync(int id);
}
