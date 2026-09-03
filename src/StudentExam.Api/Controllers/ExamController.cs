using Microsoft.AspNetCore.Mvc;
using StudentExam.Application.Common;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Services;

namespace StudentExam.Api.Controllers;

[Route("api/exams")]
public class ExamController : ApiControllerBase
{
    private readonly IExamService _examService;

    public ExamController(IExamService examService)
    {
        _examService = examService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ExamDto>>> GetExams([FromQuery] QueryParameters queryParams, CancellationToken ct)
    {
        var result = await _examService.GetExamsAsync(queryParams, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExamDto>> GetExam(int id, CancellationToken ct)
    {
        var result = await _examService.GetExamAsync(id, ct);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<ExamDto>> CreateExam(CreateExamDto dto, CancellationToken ct)
    {
        var result = await _examService.CreateExamAsync(dto, ct);
        if (!result.Succeeded)
        {
            return FromResult(result);
        }

        return CreatedAtAction(nameof(GetExam), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExam(int id, UpdateExamDto dto, CancellationToken ct)
    {
        var result = await _examService.UpdateExamAsync(id, dto, ct);
        return FromResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExam(int id, CancellationToken ct)
    {
        var result = await _examService.DeleteExamAsync(id, ct);
        return FromResult(result);
    }
}
