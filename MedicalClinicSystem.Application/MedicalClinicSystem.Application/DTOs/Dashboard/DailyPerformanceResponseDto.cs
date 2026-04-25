namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class DailyPerformanceResponseDto
    {
        public int TodayAppointments { get; set; }
        public int CompletedTodayAppointments { get; set; }
        public int CancelledTodayAppointments { get; set; }
        public int TodayVisits { get; set; }
        public int UnattendedAppointments { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal VisitConversionRate { get; set; }
    }
}