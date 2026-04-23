namespace CourseApi.DTOs.Lessons;

public record LessonDetailDto(
    int Id,
    string Title,
    string Description,
    string Content,
    int CourseId,
    bool IsCompleted,
    TestDto? Test
);

public record TestDto(
    int Id,
    string Title,
    List<QuestionDto> Questions
);

public record QuestionDto(
    int Id,
    string Text,
    int CorrectAnswerId,
    List<AnswerDto> Options
);

public record AnswerDto(int Id, string Text);

public record CompleteLessonRequest(double Score);
