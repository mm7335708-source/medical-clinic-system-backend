using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class TodayAppointmentResponseDto
    {
        public Guid AppointmentId { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;

        public string StartTime { get; set; } = string.Empty;
        public AppointmentStatus Status { get; set; }

        public string? Notes { get; set; }
    }
}