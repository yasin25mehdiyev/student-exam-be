using StudentExam.Application.Common;
using StudentExam.Application.Common.Exceptions;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Application.Interfaces.Services;
using StudentExam.Domain.Entities;

namespace StudentExam.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<StudentDto>> GetStudentsAsync(QueryParameters queryParams, CancellationToken ct = default)
    {
        var paged = await _unitOfWork.Students.GetPagedAsync(queryParams, ct);

        return new PagedResult<StudentDto>
        {
            Items = paged.Items.Select(ToDto).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };
    }

    public async Task<ServiceResult<StudentDto>> GetStudentAsync(int number, CancellationToken ct = default)
    {
        var student = await _unitOfWork.Students.GetByNumberAsync(number, ct);
        return student is null
            ? ServiceResult<StudentDto>.Fail($"Student with number '{number}' was not found.", ServiceErrorType.NotFound)
            : ServiceResult<StudentDto>.Success(ToDto(student));
    }

    public async Task<ServiceResult<StudentDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken ct = default)
    {
        if (await _unitOfWork.Students.ExistsAsync(dto.Number, ct))
        {
            return ServiceResult<StudentDto>.Fail($"Student with number '{dto.Number}' already exists.", ServiceErrorType.Conflict);
        }

        var student = new Student
        {
            Number = dto.Number,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ClassLevel = dto.ClassLevel
        };

        await _unitOfWork.Students.AddAsync(student, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<StudentDto>.Success(ToDto(student));
    }

    public async Task<ServiceResult> UpdateStudentAsync(int number, UpdateStudentDto dto, CancellationToken ct = default)
    {
        var student = await _unitOfWork.Students.GetByNumberAsync(number, ct);
        if (student is null)
        {
            return ServiceResult.Fail($"Student with number '{number}' was not found.", ServiceErrorType.NotFound);
        }

        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.ClassLevel = dto.ClassLevel;

        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteStudentAsync(int number, CancellationToken ct = default)
    {
        var student = await _unitOfWork.Students.GetByNumberAsync(number, ct);
        if (student is null)
        {
            return ServiceResult.Fail($"Student with number '{number}' was not found.", ServiceErrorType.NotFound);
        }

        _unitOfWork.Students.Remove(student);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (ForeignKeyConstraintException)
        {
            return ServiceResult.Fail($"Student '{number}' cannot be deleted because it has exam records linked to it.", ServiceErrorType.Conflict);
        }

        return ServiceResult.Success();
    }

    private static StudentDto ToDto(Student student) => new()
    {
        Number = student.Number,
        FirstName = student.FirstName,
        LastName = student.LastName,
        ClassLevel = student.ClassLevel
    };
}
