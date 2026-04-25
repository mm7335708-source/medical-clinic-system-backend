namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class VisitsSummaryResponseDto
    {
        public int TotalVisits { get; set; }
        public int TodayVisits { get; set; }
        public int ThisWeekVisits { get; set; }
        public int ThisMonthVisits { get; set; }
    }
}