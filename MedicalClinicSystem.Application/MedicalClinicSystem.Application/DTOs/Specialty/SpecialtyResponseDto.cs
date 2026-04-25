namespace MedicalClinicSystem.Application.DTOs.Specialty
{
    public class SpecialtyResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}