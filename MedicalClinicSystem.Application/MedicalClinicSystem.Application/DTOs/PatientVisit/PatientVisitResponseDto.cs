using System;

namespace MedicalClinicSystem.Application.DTOs.PatientVisit
{
    public class PatientVisitResponseDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;

        public Guid? AppointmentId { get; set; }

        public DateTime VisitDate { get; set; }

        public string ChiefComplaint { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}