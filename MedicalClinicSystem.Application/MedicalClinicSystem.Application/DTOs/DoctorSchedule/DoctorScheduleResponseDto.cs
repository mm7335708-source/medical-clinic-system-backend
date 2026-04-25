namespace MedicalClinicSystem.Application.DTOs.DoctorSchedule
{
    public class DoctorScheduleResponseDto
    {
        public Guid Id { get; set; }

        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}