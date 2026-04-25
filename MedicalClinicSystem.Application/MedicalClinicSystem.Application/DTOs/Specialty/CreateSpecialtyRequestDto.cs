namespace MedicalClinicSystem.Application.DTOs.Specialty
{
    public class CreateSpecialtyRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}