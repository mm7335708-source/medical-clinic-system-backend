using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Application.DTOs.Appointment
{
    public class AppointmentResponseDto
    {
        public Guid Id { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }
        public string StartTime { get; set; } = string.Empty;

        public string? Notes { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? CancellationReason { get; set; }
    }
}