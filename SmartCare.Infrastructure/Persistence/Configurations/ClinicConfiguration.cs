using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCare.Core.Entities;


namespace SmartCare.Infrastructure.Persistence.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.ToTable("Clinics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.HasMany(c => c.WorkingHours)
                .WithOne(wh => wh.Clinic)
                .HasForeignKey(wh => wh.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
