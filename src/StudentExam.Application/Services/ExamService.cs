using StudentExam.Application.Common;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Application.Interfaces.Services;
using StudentExam.Domain.Entities;

namespace StudentExam.Application.Services;

public class ExamService : IExamService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExamService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ExamDto>> GetExamsAsync(QueryParameters queryParams, CancellationToken ct = default)
    {
        var paged = await _unitOfWork.Exams.GetPagedAsync(queryParams, ct);

        return new PagedResult<ExamDto>
        {
            Items = paged.Items.Select(ToDto).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };
    }

    public async Task<ServiceResult<ExamDto>> GetExamAsync(int id, CancellationToken ct = default)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(id, ct);
        return exam is null
            ? ServiceResult<ExamDto>.Fail($"Exam with id '{id}' was not found.", ServiceErrorType.NotFound)
            : ServiceResult<ExamDto>.Success(ToDto(exam));
    }

    public async Task<ServiceResult<ExamDto>> CreateExamAsync(CreateExamDto dto, CancellationToken ct = default)
    {
        var courseCode = dto.CourseCode.ToUpperInvariant();

        var course = await _unitOfWork.Courses.GetByCodeAsync(courseCode, ct);
        if (course is null)
        {
            return ServiceResult<ExamDto>.Fail($"Course with code '{courseCode}' was not found.", ServiceErrorType.NotFound);
        }

        var student = await _unitOfWork.Students.GetByNumberAsync(dto.StudentNumber, ct);
        if (student is null)
        {
            return ServiceResult<ExamDto>.Fail($"Student with number '{dto.StudentNumber}' was not found.", ServiceErrorType.NotFound);
        }

        var exam = new Exam
        {
            CourseCode = courseCode,
            StudentNumber = dto.StudentNumber,
            ExamDate = dto.ExamDate,
            Score = dto.Score,
            Course = course,
            Student = student
        };

        await _unitOfWork.Exams.AddAsync(exam, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<ExamDto>.Success(ToDto(exam));
    }

    public async Task<ServiceResult> UpdateExamAsync(int id, UpdateExamDto dto, CancellationToken ct = default)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(id, ct);
        if (exam is null)
        {
            return ServiceResult.Fail($"Exam with id '{id}' was not found.", ServiceErrorType.NotFound);
        }

        exam.ExamDate = dto.ExamDate;
        exam.Score = dto.Score;

        _unitOfWork.Exams.Update(exam);
        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteExamAsync(int id, CancellationToken ct = default)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(id, ct);
        if (exam is null)
        {
            return ServiceResult.Fail($"Exam with id '{id}' was not found.", ServiceErrorType.NotFound);
        }

        _unitOfWork.Exams.Remove(exam);
        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult.Success();
    }

    private static ExamDto ToDto(Exam exam) => new()
    {
        Id = exam.Id,
        CourseCode = exam.CourseCode,
        CourseName = exam.Course.Name,
        StudentNumber = exam.StudentNumber,
        StudentFullName = $"{exam.Student.FirstName} {exam.Student.LastName}",
        ExamDate = exam.ExamDate,
        Score = exam.Score
    };
}
