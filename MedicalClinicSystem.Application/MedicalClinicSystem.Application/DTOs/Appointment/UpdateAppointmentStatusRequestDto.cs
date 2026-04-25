using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Application.DTOs.Appointment
{
    public class UpdateAppointmentStatusRequestDto
    {
        public AppointmentStatus Status { get; set; }
    }
}