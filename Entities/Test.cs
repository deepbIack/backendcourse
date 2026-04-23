namespace CourseApi.Entities;

public class Test
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int LessonId { get; set; }

    // Navigation
    public Lesson Lesson { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
