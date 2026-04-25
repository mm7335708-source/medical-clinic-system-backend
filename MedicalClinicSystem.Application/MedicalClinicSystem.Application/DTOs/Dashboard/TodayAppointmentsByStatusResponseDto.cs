using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class TodayAppointmentsByStatusResponseDto
    {
        public AppointmentStatus Status { get; set; }
        public int Count { get; set; }
    }
}