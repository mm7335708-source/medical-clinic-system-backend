namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class DashboardSummaryResponseDto
    {
        public int TotalClinics { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }

        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int CompletedAppointments { get; set; }

        public int TotalVisits { get; set; }
        public int TodayVisits { get; set; }
        public int ThisWeekVisits { get; set; }
        public int ThisMonthVisits { get; set; }
    }
}