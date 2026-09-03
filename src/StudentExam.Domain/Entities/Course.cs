namespace StudentExam.Domain.Entities;

public class Course
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ClassLevel { get; set; }
    public string TeacherFirstName { get; set; } = string.Empty;
    public string TeacherLastName { get; set; } = string.Empty;

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
