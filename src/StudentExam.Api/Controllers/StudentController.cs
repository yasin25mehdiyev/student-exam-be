using Microsoft.AspNetCore.Mvc;
using StudentExam.Application.Common;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Services;

namespace StudentExam.Api.Controllers;

[Route("api/students")]
public class StudentController : ApiControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<StudentDto>>> GetStudents([FromQuery] QueryParameters queryParams, CancellationToken ct)
    {
        var result = await _studentService.GetStudentsAsync(queryParams, ct);
        return Ok(result);
    }

    [HttpGet("{number}")]
    public async Task<ActionResult<StudentDto>> GetStudent(int number, CancellationToken ct)
    {
        var result = await _studentService.GetStudentAsync(number, ct);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentDto dto, CancellationToken ct)
    {
        var result = await _studentService.CreateStudentAsync(dto, ct);
        if (!result.Succeeded)
        {
            return FromResult(result);
        }

        return CreatedAtAction(nameof(GetStudent), new { number = result.Data!.Number }, result.Data);
    }

    [HttpPut("{number}")]
    public async Task<IActionResult> UpdateStudent(int number, UpdateStudentDto dto, CancellationToken ct)
    {
        var result = await _studentService.UpdateStudentAsync(number, dto, ct);
        return FromResult(result);
    }

    [HttpDelete("{number}")]
    public async Task<IActionResult> DeleteStudent(int number, CancellationToken ct)
    {
        var result = await _studentService.DeleteStudentAsync(number, ct);
        return FromResult(result);
    }
}
