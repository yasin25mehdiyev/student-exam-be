using StudentExam.Application.Common;
using StudentExam.Domain.Entities;

namespace StudentExam.Application.Interfaces.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByNumberAsync(int number, CancellationToken ct = default);
    Task<bool> ExistsAsync(int number, CancellationToken ct = default);
    Task<PagedResult<Student>> GetPagedAsync(QueryParameters queryParams, CancellationToken ct = default);
}
