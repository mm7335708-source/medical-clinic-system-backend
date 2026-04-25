using MedicalClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalClinicSystem.Infrastructure.Persistence.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.ToTable("Clinics");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClinicName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Latitude)
                .HasPrecision(10, 7);

            builder.Property(x => x.Longitude)
                .HasPrecision(10, 7);
        }
    }
}