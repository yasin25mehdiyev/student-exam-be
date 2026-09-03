namespace StudentExam.Domain.Entities;

public class Exam
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public int StudentNumber { get; set; }
    public DateOnly ExamDate { get; set; }
    public byte Score { get; set; }

    public Course Course { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
