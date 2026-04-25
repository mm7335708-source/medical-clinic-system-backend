namespace MedicalClinicSystem.Application.DTOs.Specialty
{
    public class UpdateSpecialtyRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}