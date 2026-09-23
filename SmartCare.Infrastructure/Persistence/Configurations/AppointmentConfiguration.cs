using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCare.Core.Entities.Appointments;

namespace SmartCare.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AppointmentDate).IsRequired();
        builder.Property(a => a.StartTime).IsRequired();
        builder.Property(a => a.EndTime).IsRequired();

        builder.Property(a => a.ConsultationFee)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(a => a.Reason).HasMaxLength(500);
        builder.Property(a => a.CancelReason).HasMaxLength(500);

        // Store Enum as String in Database
        builder.Property(a => a.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.HasOne(a => a.Doctor)
               .WithMany()
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.Status });
        builder.HasIndex(a => new { a.PatientId, a.AppointmentDate });
    }
}