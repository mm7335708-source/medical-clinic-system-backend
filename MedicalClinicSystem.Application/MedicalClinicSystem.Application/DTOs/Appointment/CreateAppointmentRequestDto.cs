namespace MedicalClinicSystem.Application.DTOs.Appointment
{
    public class CreateAppointmentRequestDto
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid ClinicId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string StartTime { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}