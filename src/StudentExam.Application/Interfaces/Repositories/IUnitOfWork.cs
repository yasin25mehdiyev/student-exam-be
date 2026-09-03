namespace StudentExam.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    IStudentRepository Students { get; }
    IExamRepository Exams { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
