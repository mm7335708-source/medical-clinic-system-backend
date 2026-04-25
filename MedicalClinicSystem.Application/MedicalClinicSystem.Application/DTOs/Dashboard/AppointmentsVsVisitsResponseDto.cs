namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class AppointmentsVsVisitsResponseDto
    {
        public int TotalAppointments { get; set; }
        public int TotalVisits { get; set; }
        public int TodayAppointments { get; set; }
        public int TodayVisits { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
    }
}