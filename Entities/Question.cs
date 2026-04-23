namespace CourseApi.Entities;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int TestId { get; set; }
    public int CorrectAnswerId { get; set; }

    // Navigation
    public Test Test { get; set; } = null!;
    public ICollection<Answer> Options { get; set; } = new List<Answer>();
}
