using System.ComponentModel.DataAnnotations;

namespace StudentExam.Application.Dtos;

public class ExamDto
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int StudentNumber { get; set; }
    public string StudentFullName { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public byte Score { get; set; }
}

public class CreateExamDto
{
    [Required, StringLength(3, MinimumLength = 3)]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    public int StudentNumber { get; set; }

    [Required]
    public DateOnly ExamDate { get; set; }

    [Range(0, 9)]
    public byte Score { get; set; }
}

public class UpdateExamDto
{
    [Required]
    public DateOnly ExamDate { get; set; }

    [Range(0, 9)]
    public byte Score { get; set; }
}
