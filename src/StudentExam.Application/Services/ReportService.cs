using StudentExam.Application.Common;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Application.Interfaces.Services;

namespace StudentExam.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<StudentReportDto>> GetStudentReportAsync(int studentNumber, CancellationToken ct = default)
    {
        var student = await _unitOfWork.Students.GetByNumberAsync(studentNumber, ct);
        if (student is null)
        {
            return ServiceResult<StudentReportDto>.Fail($"Student with number '{studentNumber}' was not found.", ServiceErrorType.NotFound);
        }

        var exams = await _unitOfWork.Exams.GetByStudentNumberAsync(studentNumber, ct);
        var studentFullName = $"{student.FirstName} {student.LastName}";

        var examDtos = exams.Select(e => new ExamDto
        {
            Id = e.Id,
            CourseCode = e.CourseCode,
            CourseName = e.Course.Name,
            StudentNumber = e.StudentNumber,
            StudentFullName = studentFullName,
            ExamDate = e.ExamDate,
            Score = e.Score
        }).ToList();

        var report = new StudentReportDto
        {
            StudentNumber = student.Number,
            FullName = studentFullName,
            ClassLevel = student.ClassLevel,
            Exams = examDtos,
            AverageScore = examDtos.Count > 0 ? Math.Round(examDtos.Average(e => e.Score), 2) : null
        };

        return ServiceResult<StudentReportDto>.Success(report);
    }

    public async Task<ServiceResult<List<ClassAverageDto>>> GetClassAveragesAsync(string? courseCode, CancellationToken ct = default)
    {
        var normalizedCode = string.IsNullOrWhiteSpace(courseCode) ? null : courseCode.ToUpperInvariant();

        if (normalizedCode is not null && !await _unitOfWork.Courses.ExistsAsync(normalizedCode, ct))
        {
            return ServiceResult<List<ClassAverageDto>>.Fail($"Course with code '{normalizedCode}' was not found.", ServiceErrorType.NotFound);
        }

        var averages = await _unitOfWork.Exams.GetClassAveragesAsync(normalizedCode, ct);

        foreach (var average in averages)
        {
            average.AverageScore = Math.Round(average.AverageScore, 2);
        }

        return ServiceResult<List<ClassAverageDto>>.Success(averages);
    }

    public async Task<ServiceResult<SystemSummaryDto>> GetSummaryAsync(CancellationToken ct = default)
    {
        var summary = new SystemSummaryDto
        {
            TotalCourses = await _unitOfWork.Courses.CountAsync(ct),
            TotalStudents = await _unitOfWork.Students.CountAsync(ct),
            TotalExams = await _unitOfWork.Exams.CountAsync(ct)
        };

        return ServiceResult<SystemSummaryDto>.Success(summary);
    }
}
