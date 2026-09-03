using System.ComponentModel.DataAnnotations;

namespace StudentExam.Application.Dtos;

public class StudentDto
{
    public int Number { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int ClassLevel { get; set; }
}

public class CreateStudentDto
{
    [Required, Range(1, 99999)]
    public int Number { get; set; }

    [Required, StringLength(30)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string LastName { get; set; } = string.Empty;

    [Range(1, 11)]
    public int ClassLevel { get; set; }
}

public class UpdateStudentDto
{
    [Required, StringLength(30)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string LastName { get; set; } = string.Empty;

    [Range(1, 11)]
    public int ClassLevel { get; set; }
}
