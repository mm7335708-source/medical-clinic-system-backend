namespace MedicalClinicSystem.Application.DTOs.Dashboard
{
    public class ClinicActivityResponseDto
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public int AppointmentsCount { get; set; }
    }
}