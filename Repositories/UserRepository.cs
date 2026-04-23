using Microsoft.EntityFrameworkCore;
using CourseApi.Data;
using CourseApi.Entities;
using CourseApi.Repositories.Interfaces;

namespace CourseApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(int id) =>
        await _db.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _db.Users.AnyAsync(u => u.Email == email.ToLower());

    public async Task<User> CreateAsync(User user)
    {
        user.Email = user.Email.ToLower();
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
