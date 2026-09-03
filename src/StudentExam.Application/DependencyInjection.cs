using Microsoft.Extensions.DependencyInjection;
using StudentExam.Application.Interfaces.Services;
using StudentExam.Application.Services;

namespace StudentExam.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
