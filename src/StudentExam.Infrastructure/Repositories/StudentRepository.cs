using Microsoft.EntityFrameworkCore;
using StudentExam.Application.Common;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Domain.Entities;
using StudentExam.Infrastructure.Persistence;

namespace StudentExam.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByNumberAsync(int number, CancellationToken ct = default) =>
        await _context.Students.FindAsync(new object?[] { number }, ct);

    public async Task<bool> ExistsAsync(int number, CancellationToken ct = default) =>
        await _context.Students.AnyAsync(s => s.Number == number, ct);

    public async Task<PagedResult<Student>> GetPagedAsync(QueryParameters queryParams, CancellationToken ct = default)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var term = queryParams.Search.Trim();
            query = query.Where(s => s.FirstName.Contains(term) || s.LastName.Contains(term));
        }

        query = (queryParams.SortBy?.ToLowerInvariant(), queryParams.SortDirection) switch
        {
            ("firstname", SortDirection.Desc) => query.OrderByDescending(s => s.FirstName),
            ("firstname", _) => query.OrderBy(s => s.FirstName),
            ("lastname", SortDirection.Desc) => query.OrderByDescending(s => s.LastName),
            ("lastname", _) => query.OrderBy(s => s.LastName),
            ("classlevel", SortDirection.Desc) => query.OrderByDescending(s => s.ClassLevel),
            ("classlevel", _) => query.OrderBy(s => s.ClassLevel),
            (_, SortDirection.Desc) => query.OrderByDescending(s => s.Number),
            _ => query.OrderBy(s => s.Number)
        };

        return await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize, ct);
    }

    public async Task AddAsync(Student entity, CancellationToken ct = default) =>
        await _context.Students.AddAsync(entity, ct);

    public void Update(Student entity) => _context.Students.Update(entity);

    public void Remove(Student entity) => _context.Students.Remove(entity);
}
