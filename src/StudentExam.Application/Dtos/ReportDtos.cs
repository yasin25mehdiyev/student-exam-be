namespace StudentExam.Application.Dtos;

public class StudentReportDto
{
    public int StudentNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int ClassLevel { get; set; }
    public List<ExamDto> Exams { get; set; } = new();
    public double? AverageScore { get; set; }
}

public class ClassAverageDto
{
    public int ClassLevel { get; set; }
    public string? CourseCode { get; set; }
    public double AverageScore { get; set; }
    public int ExamCount { get; set; }
}
