namespace MedicalClinicSystem.Application.DTOs.Doctor
{
    public class DoctorResponseDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }

        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;

        public Guid SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
    }
}