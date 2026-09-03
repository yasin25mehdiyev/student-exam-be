using Microsoft.EntityFrameworkCore;
using StudentExam.Application.Common.Exceptions;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Infrastructure.Persistence;

namespace StudentExam.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context, ICourseRepository courses, IStudentRepository students, IExamRepository exams)
    {
        _context = context;
        Courses = courses;
        Students = students;
        Exams = exams;
    }

    public ICourseRepository Courses { get; }
    public IStudentRepository Students { get; }
    public IExamRepository Exams { get; }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            throw new ForeignKeyConstraintException(
                "The operation could not be completed because of a related record constraint.", ex);
        }
    }
}
