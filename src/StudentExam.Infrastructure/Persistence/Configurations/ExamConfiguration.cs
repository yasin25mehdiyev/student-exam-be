using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentExam.Domain.Entities;

namespace StudentExam.Infrastructure.Persistence.Configurations;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CourseCode).HasColumnType("char(3)").IsRequired();
        builder.Property(e => e.Score).HasColumnType("tinyint");
        builder.Property(e => e.ExamDate).HasColumnType("date");

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Exams)
            .HasForeignKey(e => e.CourseCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Exams)
            .HasForeignKey(e => e.StudentNumber)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint("CK_Exam_Score_Range", "[Score] BETWEEN 0 AND 9"));
    }
}
