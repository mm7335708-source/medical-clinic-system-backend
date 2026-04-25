namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class DoctorSummaryResponseDto
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }

        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
    }
}