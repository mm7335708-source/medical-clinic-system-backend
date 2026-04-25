using MedicalClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalClinicSystem.Infrastructure.Persistence.Configurations
{
    public class PatientVisitConfiguration : IEntityTypeConfiguration<PatientVisit>
    {
        public void Configure(EntityTypeBuilder<PatientVisit> builder)
        {
            builder.ToTable("PatientVisits");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.VisitDate)
                .IsRequired();

            builder.Property(x => x.ChiefComplaint)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Diagnosis)
                .HasMaxLength(2000);

            builder.Property(x => x.TreatmentPlan)
                .HasMaxLength(2000);

            builder.Property(x => x.Prescription)
                .HasMaxLength(2000);

            builder.Property(x => x.Notes)
                .HasMaxLength(3000);

            builder.HasOne(x => x.Patient)
                .WithMany(x => x.PatientVisits)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Doctor)
                .WithMany(x => x.PatientVisits)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Clinic)
                .WithMany(x => x.PatientVisits)
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Appointment)
                .WithOne(x => x.PatientVisit)
                .HasForeignKey<PatientVisit>(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => x.AppointmentId)
                .IsUnique();
        }
    }
}