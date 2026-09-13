using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCare.Core.Entities.Docotr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Infrastructure.Persistence.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Biography)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(d => d.YearsOfExperience)
                .IsRequired();

            builder.Property(d => d.ConsultationFee)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(d => d.IsActive)
                .IsRequired();

            // Doctor -> ApplicationUser (1 : 1)
            builder.HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor -> Specialty (Many : 1)
            builder.HasOne(d => d.Specialty)
                .WithMany()
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor -> Clinic (Many : 1)
            builder.HasOne(d => d.Clinic)
                .WithMany()
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // A user can have only one doctor profile
            builder.HasIndex(d => d.UserId)
                .IsUnique();

            // Useful indexes for filtering
            builder.HasIndex(d => d.SpecialtyId);

            builder.HasIndex(d => d.ClinicId);

            builder.HasIndex(d => d.IsActive);

            builder.HasIndex(d => new
            {
                d.SpecialtyId,
                d.IsActive
            });

            builder.HasIndex(d => new
            {
                d.ClinicId,
                d.IsActive
            });
        }
    }
}
