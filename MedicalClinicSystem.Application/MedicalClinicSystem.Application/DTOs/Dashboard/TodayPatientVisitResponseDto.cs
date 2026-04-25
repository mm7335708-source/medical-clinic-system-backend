namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class TodayPatientVisitResponseDto
    {
        public Guid VisitId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string ChiefComplaint { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
    }
}