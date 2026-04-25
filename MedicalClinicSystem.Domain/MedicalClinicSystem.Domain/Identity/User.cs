using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Domain.Entities.Identity
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public Guid RoleId { get; set; }
        public Guid? DoctorId { get; set; }

        public Role Role { get; set; } = null!;
        public Doctor? Doctor { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
