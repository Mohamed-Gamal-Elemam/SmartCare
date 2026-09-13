using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCare.Core.Entities.Climic;
using System;


namespace SmartCare.Infrastructure.Persistence.Configurations
{
    public class ClinicWorkingHourConfiguration : IEntityTypeConfiguration<ClinicWorkingHour>
    {
        public void Configure(EntityTypeBuilder<ClinicWorkingHour> builder)
        {
            builder.ToTable("ClinicWorkingHours");

            builder.HasKey(wh => wh.Id);

            builder.Property(wh => wh.DayOfWeek)
            .HasConversion<string>()
            .IsRequired();

            builder.Property(wh => wh.OpenTime)
                .HasColumnType("time");

            builder.Property(wh => wh.CloseTime)
                .HasColumnType("time");

            builder.Property(wh => wh.IsClosed)
                .IsRequired();

            builder.HasIndex(wh => new
            {
                wh.ClinicId,
                wh.DayOfWeek
            })
            .IsUnique();

            builder.HasOne(wh => wh.Clinic)
                .WithMany(c => c.WorkingHours)
                .HasForeignKey(wh => wh.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
