using StudentExam.Application.Common;
using StudentExam.Application.Common.Exceptions;
using StudentExam.Application.Dtos;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Application.Interfaces.Services;
using StudentExam.Domain.Entities;

namespace StudentExam.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CourseDto>> GetCoursesAsync(QueryParameters queryParams, CancellationToken ct = default)
    {
        var paged = await _unitOfWork.Courses.GetPagedAsync(queryParams, ct);

        return new PagedResult<CourseDto>
        {
            Items = paged.Items.Select(ToDto).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };
    }

    public async Task<ServiceResult<CourseDto>> GetCourseAsync(string code, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Courses.GetByCodeAsync(code, ct);
        return course is null
            ? ServiceResult<CourseDto>.Fail($"Course with code '{code}' was not found.", ServiceErrorType.NotFound)
            : ServiceResult<CourseDto>.Success(ToDto(course));
    }

    public async Task<ServiceResult<CourseDto>> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default)
    {
        var code = dto.Code.ToUpperInvariant();

        if (await _unitOfWork.Courses.ExistsAsync(code, ct))
        {
            return ServiceResult<CourseDto>.Fail($"Course with code '{code}' already exists.", ServiceErrorType.Conflict);
        }

        var course = new Course
        {
            Code = code,
            Name = dto.Name,
            ClassLevel = dto.ClassLevel,
            TeacherFirstName = dto.TeacherFirstName,
            TeacherLastName = dto.TeacherLastName
        };

        await _unitOfWork.Courses.AddAsync(course, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<CourseDto>.Success(ToDto(course));
    }

    public async Task<ServiceResult> UpdateCourseAsync(string code, UpdateCourseDto dto, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Courses.GetByCodeAsync(code, ct);
        if (course is null)
        {
            return ServiceResult.Fail($"Course with code '{code}' was not found.", ServiceErrorType.NotFound);
        }

        course.Name = dto.Name;
        course.ClassLevel = dto.ClassLevel;
        course.TeacherFirstName = dto.TeacherFirstName;
        course.TeacherLastName = dto.TeacherLastName;

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteCourseAsync(string code, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Courses.GetByCodeAsync(code, ct);
        if (course is null)
        {
            return ServiceResult.Fail($"Course with code '{code}' was not found.", ServiceErrorType.NotFound);
        }

        _unitOfWork.Courses.Remove(course);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (ForeignKeyConstraintException)
        {
            return ServiceResult.Fail($"Course '{code}' cannot be deleted because it has exam records linked to it.", ServiceErrorType.Conflict);
        }

        return ServiceResult.Success();
    }

    private static CourseDto ToDto(Course course) => new()
    {
        Code = course.Code,
        Name = course.Name,
        ClassLevel = course.ClassLevel,
        TeacherFirstName = course.TeacherFirstName,
        TeacherLastName = course.TeacherLastName
    };
}
