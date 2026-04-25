namespace MedicalClinicSystem.Application.DTOs.Patient
{
    public class PatientResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
    }
}