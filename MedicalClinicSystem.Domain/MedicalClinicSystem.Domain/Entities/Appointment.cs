using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; } = default!;
        public virtual PatientVisit? PatientVisit { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
    }
}