using StudentExam.Application.Common;
using StudentExam.Application.Dtos;

namespace StudentExam.Application.Interfaces.Services;

public interface IReportService
{
    Task<ServiceResult<StudentReportDto>> GetStudentReportAsync(int studentNumber, CancellationToken ct = default);
    Task<ServiceResult<List<ClassAverageDto>>> GetClassAveragesAsync(string? courseCode, CancellationToken ct = default);
    Task<ServiceResult<SystemSummaryDto>> GetSummaryAsync(CancellationToken ct = default);
}
