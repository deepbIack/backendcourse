using Microsoft.EntityFrameworkCore;
using CourseApi.Entities;

namespace CourseApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<UserProgress> UserProgresses => Set<UserProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.Name).IsRequired().HasMaxLength(128);
            e.Property(u => u.PasswordHash).IsRequired();
        });

        // Course
        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Title).IsRequired().HasMaxLength(256);

            e.HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)   // явно указываем правильный FK
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Lesson → Course (one-to-many)
        modelBuilder.Entity<Lesson>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Title).IsRequired().HasMaxLength(256);
            e.HasOne(l => l.Course)
             .WithMany(c => c.Lessons)
             .HasForeignKey(l => l.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Test → Lesson (one-to-one)
        modelBuilder.Entity<Test>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasOne(t => t.Lesson)
             .WithOne(l => l.Test)
             .HasForeignKey<Test>(t => t.LessonId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Question → Test (one-to-many)
        modelBuilder.Entity<Question>(e =>
        {
            e.HasKey(q => q.Id);
            e.HasOne(q => q.Test)
             .WithMany(t => t.Questions)
             .HasForeignKey(q => q.TestId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Answer → Question (one-to-many)
        modelBuilder.Entity<Answer>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.Question)
             .WithMany(q => q.Options)
             .HasForeignKey(a => a.QuestionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // UserProgress → User + Lesson
        modelBuilder.Entity<UserProgress>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.UserId, p.LessonId }).IsUnique();
            e.HasOne(p => p.User)
             .WithMany(u => u.Progresses)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Lesson)
             .WithMany(l => l.Progresses)
             .HasForeignKey(p => p.LessonId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
