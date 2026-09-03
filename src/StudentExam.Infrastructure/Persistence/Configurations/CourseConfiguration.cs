using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentExam.Domain.Entities;

namespace StudentExam.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Code);
        builder.Property(c => c.Code).HasColumnType("char(3)").ValueGeneratedNever();
        builder.Property(c => c.Name).HasMaxLength(30).IsRequired();
        builder.Property(c => c.TeacherFirstName).HasMaxLength(20).IsRequired();
        builder.Property(c => c.TeacherLastName).HasMaxLength(20).IsRequired();
        builder.Property(c => c.ClassLevel).HasColumnType("tinyint");
    }
}
