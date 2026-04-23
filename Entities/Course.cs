namespace CourseApi.Entities;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoverImage { get; set; }
    public int? CreatedByUserId { get; set; }   // null = системный курс
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    public User? CreatedBy { get; set; }
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
