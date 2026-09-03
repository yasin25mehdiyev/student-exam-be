using StudentExam.Application.Common;
using StudentExam.Application.Dtos;

namespace StudentExam.Application.Interfaces.Services;

public interface IStudentService
{
    Task<PagedResult<StudentDto>> GetStudentsAsync(QueryParameters queryParams, CancellationToken ct = default);
    Task<ServiceResult<StudentDto>> GetStudentAsync(int number, CancellationToken ct = default);
    Task<ServiceResult<StudentDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken ct = default);
    Task<ServiceResult> UpdateStudentAsync(int number, UpdateStudentDto dto, CancellationToken ct = default);
    Task<ServiceResult> DeleteStudentAsync(int number, CancellationToken ct = default);
}
