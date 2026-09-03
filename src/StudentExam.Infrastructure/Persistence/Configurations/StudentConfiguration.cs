using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentExam.Domain.Entities;

namespace StudentExam.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Number);
        builder.Property(s => s.Number).ValueGeneratedNever();
        builder.Property(s => s.FirstName).HasMaxLength(30).IsRequired();
        builder.Property(s => s.LastName).HasMaxLength(30).IsRequired();
        builder.Property(s => s.ClassLevel).HasColumnType("tinyint");
    }
}
