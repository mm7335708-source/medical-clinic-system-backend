namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class BusyClinicTodayResponseDto
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public int VisitsCount { get; set; }
    }
}