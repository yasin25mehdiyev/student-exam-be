using Microsoft.EntityFrameworkCore;
using StudentExam.Application.Common;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Domain.Entities;
using StudentExam.Infrastructure.Persistence;

namespace StudentExam.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await _context.Courses.FindAsync(new object?[] { code }, ct);

    public async Task<bool> ExistsAsync(string code, CancellationToken ct = default) =>
        await _context.Courses.AnyAsync(c => c.Code == code, ct);

    public async Task<PagedResult<Course>> GetPagedAsync(QueryParameters queryParams, CancellationToken ct = default)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var term = queryParams.Search.Trim();
            query = query.Where(c =>
                c.Code.Contains(term) ||
                c.Name.Contains(term) ||
                c.TeacherFirstName.Contains(term) ||
                c.TeacherLastName.Contains(term));
        }

        query = (queryParams.SortBy?.ToLowerInvariant(), queryParams.SortDirection) switch
        {
            ("name", SortDirection.Desc) => query.OrderByDescending(c => c.Name),
            ("name", _) => query.OrderBy(c => c.Name),
            ("classlevel", SortDirection.Desc) => query.OrderByDescending(c => c.ClassLevel),
            ("classlevel", _) => query.OrderBy(c => c.ClassLevel),
            ("teacherfirstname", SortDirection.Desc) => query.OrderByDescending(c => c.TeacherFirstName),
            ("teacherfirstname", _) => query.OrderBy(c => c.TeacherFirstName),
            ("teacherlastname", SortDirection.Desc) => query.OrderByDescending(c => c.TeacherLastName),
            ("teacherlastname", _) => query.OrderBy(c => c.TeacherLastName),
            (_, SortDirection.Desc) => query.OrderByDescending(c => c.Code),
            _ => query.OrderBy(c => c.Code)
        };

        return await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize, ct);
    }

    public async Task AddAsync(Course entity, CancellationToken ct = default) =>
        await _context.Courses.AddAsync(entity, ct);

    public void Update(Course entity) => _context.Courses.Update(entity);

    public void Remove(Course entity) => _context.Courses.Remove(entity);
}
