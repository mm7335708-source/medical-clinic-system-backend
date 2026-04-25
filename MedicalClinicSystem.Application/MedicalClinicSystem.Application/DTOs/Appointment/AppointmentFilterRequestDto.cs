using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Application.DTOs.Appointment
{
    public class AppointmentFilterRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public Guid? DoctorId { get; set; }
        public Guid? ClinicId { get; set; }
        public AppointmentStatus? Status { get; set; }
        public DateTime? Date { get; set; }
    }
}