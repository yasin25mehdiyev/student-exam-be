namespace StudentExam.Domain.Entities;

public class Student
{
    public int Number { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int ClassLevel { get; set; }

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
