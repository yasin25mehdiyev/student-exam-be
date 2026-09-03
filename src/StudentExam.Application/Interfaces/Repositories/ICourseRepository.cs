using StudentExam.Application.Common;
using StudentExam.Domain.Entities;

namespace StudentExam.Application.Interfaces.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> ExistsAsync(string code, CancellationToken ct = default);
    Task<PagedResult<Course>> GetPagedAsync(QueryParameters queryParams, CancellationToken ct = default);
}
