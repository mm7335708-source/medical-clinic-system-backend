using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class UpcomingAppointmentResponseDto
    {
        public Guid AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string StartTime { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; }
    }
}