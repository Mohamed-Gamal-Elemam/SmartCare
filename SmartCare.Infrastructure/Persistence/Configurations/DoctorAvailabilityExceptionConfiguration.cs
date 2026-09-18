using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCare.Core.Entities.DoctorAvailabilitys;


namespace SmartCare.Infrastructure.Persistence.Configurations
{
    public class DoctorAvailabilityExceptionConfiguration
       : IEntityTypeConfiguration<DoctorAvailabilityException>
    {
        public void Configure(EntityTypeBuilder<DoctorAvailabilityException> builder)
        {
            builder.ToTable("DoctorAvailabilityExceptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoctorId)
                .IsRequired();

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.IsUnavailable)
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            builder.HasOne(x => x.Doctor)
                .WithMany()
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.DoctorId,
                x.Date
            });
        }
    }
}
