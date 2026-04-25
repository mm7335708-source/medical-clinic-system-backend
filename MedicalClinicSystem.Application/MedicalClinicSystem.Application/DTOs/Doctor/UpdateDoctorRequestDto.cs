namespace MedicalClinicSystem.Application.DTOs.Doctor
{
    public class UpdateDoctorRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }

        public Guid ClinicId { get; set; }
        public Guid SpecialtyId { get; set; }
    }
}