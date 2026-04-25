namespace MedicalClinicSystem.Application.DTOs.DoctorSchedule
{
    public class UpdateDoctorScheduleRequestDto
    {
        public Guid DoctorId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}