namespace MedicalClinicSystem.Application.DTOs.Identity
{
    public class UpdateUserRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid RoleId { get; set; }
        public Guid? DoctorId { get; set; }
        public bool IsActive { get; set; }
    }
}
