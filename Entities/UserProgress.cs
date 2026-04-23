namespace CourseApi.Entities;

public class UserProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LessonId { get; set; }
    public double Score { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}
