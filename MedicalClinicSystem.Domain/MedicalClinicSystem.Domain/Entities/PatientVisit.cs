using System;

namespace MedicalClinicSystem.Domain.Entities
{
    public class PatientVisit : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ClinicId { get; set; }
        public Guid? AppointmentId { get; set; }

        public DateTime VisitDate { get; set; }

        public string ChiefComplaint { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Clinic Clinic { get; set; } = null!;
        public virtual Appointment? Appointment { get; set; }
    }
}