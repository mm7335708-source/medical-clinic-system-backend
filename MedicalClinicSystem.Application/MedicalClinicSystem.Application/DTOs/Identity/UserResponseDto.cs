namespace MedicalClinicSystem.Application.DTOs.Identity
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public Guid? DoctorId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
