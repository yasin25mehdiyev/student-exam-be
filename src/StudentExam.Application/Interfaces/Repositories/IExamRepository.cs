using StudentExam.Application.Common;
using StudentExam.Application.Dtos;
using StudentExam.Domain.Entities;

namespace StudentExam.Application.Interfaces.Repositories;

public interface IExamRepository : IRepository<Exam>
{
    /// <summary>Loads the exam together with its Course and Student navigation properties.</summary>
    Task<Exam?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<PagedResult<Exam>> GetPagedAsync(QueryParameters queryParams, CancellationToken ct = default);

    /// <summary>All exams for a student, ordered by date, with Course/Student loaded.</summary>
    Task<List<Exam>> GetByStudentNumberAsync(int studentNumber, CancellationToken ct = default);

    /// <summary>Average score grouped by the student's class level, optionally restricted to one course.</summary>
    Task<List<ClassAverageDto>> GetClassAveragesAsync(string? courseCode, CancellationToken ct = default);
}
