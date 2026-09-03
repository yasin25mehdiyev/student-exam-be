using System.ComponentModel.DataAnnotations;

namespace StudentExam.Application.Dtos;

public class CourseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ClassLevel { get; set; }
    public string TeacherFirstName { get; set; } = string.Empty;
    public string TeacherLastName { get; set; } = string.Empty;
}

public class CreateCourseDto
{
    [Required, StringLength(3, MinimumLength = 3)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 11)]
    public int ClassLevel { get; set; }

    [Required, StringLength(20)]
    public string TeacherFirstName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string TeacherLastName { get; set; } = string.Empty;
}

public class UpdateCourseDto
{
    [Required, StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 11)]
    public int ClassLevel { get; set; }

    [Required, StringLength(20)]
    public string TeacherFirstName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string TeacherLastName { get; set; } = string.Empty;
}
