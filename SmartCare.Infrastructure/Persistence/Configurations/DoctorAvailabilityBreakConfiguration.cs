using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCare.Core.Entities.DoctorAvailabilitys;


namespace SmartCare.Infrastructure.Persistence.Configurations
{
    public class DoctorAvailabilityBreakConfiguration
      : IEntityTypeConfiguration<DoctorAvailabilityBreak>
    {
        public void Configure(EntityTypeBuilder<DoctorAvailabilityBreak> builder)
        {
            builder.ToTable("DoctorAvailabilityBreaks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoctorAvailabilityId)
                .IsRequired();

            builder.Property(x => x.StartTime)
                .IsRequired();

            builder.Property(x => x.EndTime)
                .IsRequired();

            builder.HasOne(x => x.DoctorAvailability)
                .WithMany(x => x.Breaks)
                .HasForeignKey(x => x.DoctorAvailabilityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.DoctorAvailabilityId);
        }
    }
}
