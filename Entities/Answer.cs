namespace CourseApi.Entities;

public class Answer
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int QuestionId { get; set; }

    // Navigation
    public Question Question { get; set; } = null!;
}
