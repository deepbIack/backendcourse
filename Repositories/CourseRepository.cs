using Microsoft.EntityFrameworkCore;
using CourseApi.Data;
using CourseApi.Entities;
using CourseApi.Repositories.Interfaces;

namespace CourseApi.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;
    public CourseRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Course>> GetAllAsync() =>
        await _db.Courses.OrderBy(c => c.CreatedAt).ToListAsync();

    public async Task<IEnumerable<Course>> GetByUserIdAsync(int userId) =>
        await _db.Courses
            .Where(c => c.CreatedByUserId == userId)
            .Include(c => c.Lessons)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

    public async Task<Course?> GetByIdWithLessonsAsync(int id) =>
        await _db.Courses
            .Include(c => c.Lessons.OrderBy(l => l.Order))
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Course?> GetByIdAsync(int id) =>
        await _db.Courses.FindAsync(id);

    public async Task<Course> CreateAsync(Course course)
    {
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return course;
    }

    public async Task<Course> UpdateAsync(Course course)
    {
        _db.Courses.Update(course);
        await _db.SaveChangesAsync();
        return course;
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course is not null)
        {
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }
    }
}
