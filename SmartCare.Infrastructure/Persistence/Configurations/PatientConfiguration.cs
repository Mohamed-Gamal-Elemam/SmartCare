using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCare.Core.Entities.Patients;

namespace SmartCare.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Gender)
               .HasMaxLength(10);

        builder.Property(p => p.BloodGroup)
               .HasMaxLength(5);

        builder.Property(p => p.Address)
               .HasMaxLength(250);

        builder.Property(p => p.EmergencyContact)
               .HasMaxLength(20);

        builder.HasOne(p => p.User)
               .WithOne(u => u.Patient)
               .HasForeignKey<Patient>(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}