namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class BusyDoctorTodayResponseDto
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int AppointmentsCount { get; set; }
    }
}