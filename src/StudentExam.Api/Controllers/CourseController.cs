using Microsoft.AspNetCore.Mvc;
using StudentExam.Application.Common;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Services;

namespace StudentExam.Api.Controllers;

[Route("api/courses")]
public class CourseController : ApiControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CourseDto>>> GetCourses([FromQuery] QueryParameters queryParams, CancellationToken ct)
    {
        var result = await _courseService.GetCoursesAsync(queryParams, ct);
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<CourseDto>> GetCourse(string code, CancellationToken ct)
    {
        var result = await _courseService.GetCourseAsync(code, ct);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto dto, CancellationToken ct)
    {
        var result = await _courseService.CreateCourseAsync(dto, ct);
        if (!result.Succeeded)
        {
            return FromResult(result);
        }

        return CreatedAtAction(nameof(GetCourse), new { code = result.Data!.Code }, result.Data);
    }

    [HttpPut("{code}")]
    public async Task<IActionResult> UpdateCourse(string code, UpdateCourseDto dto, CancellationToken ct)
    {
        var result = await _courseService.UpdateCourseAsync(code, dto, ct);
        return FromResult(result);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> DeleteCourse(string code, CancellationToken ct)
    {
        var result = await _courseService.DeleteCourseAsync(code, ct);
        return FromResult(result);
    }
}
