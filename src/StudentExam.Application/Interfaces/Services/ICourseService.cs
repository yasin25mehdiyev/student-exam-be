using StudentExam.Application.Common;
using StudentExam.Application.Dtos;

namespace StudentExam.Application.Interfaces.Services;

public interface ICourseService
{
    Task<PagedResult<CourseDto>> GetCoursesAsync(QueryParameters queryParams, CancellationToken ct = default);
    Task<ServiceResult<CourseDto>> GetCourseAsync(string code, CancellationToken ct = default);
    Task<ServiceResult<CourseDto>> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default);
    Task<ServiceResult> UpdateCourseAsync(string code, UpdateCourseDto dto, CancellationToken ct = default);
    Task<ServiceResult> DeleteCourseAsync(string code, CancellationToken ct = default);
}
