using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentExam.Application.Interfaces.Repositories;
using StudentExam.Infrastructure.Persistence;
using StudentExam.Infrastructure.Repositories;

namespace StudentExam.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
