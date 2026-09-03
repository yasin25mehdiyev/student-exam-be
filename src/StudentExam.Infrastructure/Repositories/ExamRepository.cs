using Microsoft.EntityFrameworkCore;
using StudentExam.Application.Common;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Domain.Entities;
using StudentExam.Infrastructure.Persistence;

namespace StudentExam.Infrastructure.Repositories;

public class ExamRepository : IExamRepository
{
    private readonly AppDbContext _context;

    public ExamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Exam?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _context.Exams
            .Include(e => e.Course)
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<Exam>> GetPagedAsync(QueryParameters queryParams, CancellationToken ct = default)
    {
        var query = _context.Exams
            .Include(e => e.Course)
            .Include(e => e.Student)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var term = queryParams.Search.Trim();
            query = query.Where(e =>
                e.CourseCode.Contains(term) ||
                e.Course.Name.Contains(term) ||
                e.Student.FirstName.Contains(term) ||
                e.Student.LastName.Contains(term));
        }

        query = (queryParams.SortBy?.ToLowerInvariant(), queryParams.SortDirection) switch
        {
            ("score", SortDirection.Desc) => query.OrderByDescending(e => e.Score),
            ("score", _) => query.OrderBy(e => e.Score),
            ("coursecode", SortDirection.Desc) => query.OrderByDescending(e => e.CourseCode),
            ("coursecode", _) => query.OrderBy(e => e.CourseCode),
            ("coursename", SortDirection.Desc) => query.OrderByDescending(e => e.Course.Name),
            ("coursename", _) => query.OrderBy(e => e.Course.Name),
            ("studentnumber", SortDirection.Desc) => query.OrderByDescending(e => e.StudentNumber),
            ("studentnumber", _) => query.OrderBy(e => e.StudentNumber),
            ("studentname", SortDirection.Desc) => query.OrderByDescending(e => e.Student.LastName).ThenByDescending(e => e.Student.FirstName),
            ("studentname", _) => query.OrderBy(e => e.Student.LastName).ThenBy(e => e.Student.FirstName),
            (_, SortDirection.Desc) => query.OrderByDescending(e => e.ExamDate),
            _ => query.OrderBy(e => e.ExamDate)
        };

        return await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize, ct);
    }

    public async Task<List<Exam>> GetByStudentNumberAsync(int studentNumber, CancellationToken ct = default) =>
        await _context.Exams
            .Include(e => e.Course)
            .Where(e => e.StudentNumber == studentNumber)
            .OrderBy(e => e.ExamDate)
            .ToListAsync(ct);

    public async Task<List<ClassAverageDto>> GetClassAveragesAsync(string? courseCode, CancellationToken ct = default)
    {
        var query = _context.Exams.AsQueryable();

        if (courseCode is not null)
        {
            query = query.Where(e => e.CourseCode == courseCode);
        }

        return await query
            .GroupBy(e => e.Student.ClassLevel)
            .Select(g => new ClassAverageDto
            {
                ClassLevel = g.Key,
                CourseCode = courseCode,
                AverageScore = g.Average(e => (double)e.Score),
                ExamCount = g.Count()
            })
            .OrderBy(a => a.ClassLevel)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Exam entity, CancellationToken ct = default) =>
        await _context.Exams.AddAsync(entity, ct);

    public void Update(Exam entity) => _context.Exams.Update(entity);

    public void Remove(Exam entity) => _context.Exams.Remove(entity);
}
