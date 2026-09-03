using Microsoft.AspNetCore.Mvc;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Services;

namespace StudentExam.Api.Controllers;

[Route("api/reports")]
public class ReportController : ApiControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("students/{number}")]
    public async Task<ActionResult<StudentReportDto>> GetStudentReport(int number, CancellationToken ct)
    {
        var result = await _reportService.GetStudentReportAsync(number, ct);
        return FromResult(result);
    }

    [HttpGet("class-averages")]
    public async Task<ActionResult<List<ClassAverageDto>>> GetClassAverages([FromQuery] string? courseCode, CancellationToken ct)
    {
        var result = await _reportService.GetClassAveragesAsync(courseCode, ct);
        return FromResult(result);
    }
}
