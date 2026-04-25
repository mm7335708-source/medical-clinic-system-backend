namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class DoctorVisitsSummaryResponseDto
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int TotalVisits { get; set; }
        public int TodayVisits { get; set; }
        public int ThisWeekVisits { get; set; }
        public int ThisMonthVisits { get; set; }
    }
}