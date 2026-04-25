namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class ClinicVisitsSummaryResponseDto
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public int TotalVisits { get; set; }
        public int TodayVisits { get; set; }
        public int ThisWeekVisits { get; set; }
        public int ThisMonthVisits { get; set; }
    }
}