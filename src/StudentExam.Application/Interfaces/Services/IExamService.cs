using StudentExam.Application.Common;
using StudentExam.Application.Dtos;

namespace StudentExam.Application.Interfaces.Services;

public interface IExamService
{
    Task<PagedResult<ExamDto>> GetExamsAsync(QueryParameters queryParams, CancellationToken ct = default);
    Task<ServiceResult<ExamDto>> GetExamAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<ExamDto>> CreateExamAsync(CreateExamDto dto, CancellationToken ct = default);
    Task<ServiceResult> UpdateExamAsync(int id, UpdateExamDto dto, CancellationToken ct = default);
    Task<ServiceResult> DeleteExamAsync(int id, CancellationToken ct = default);
}
