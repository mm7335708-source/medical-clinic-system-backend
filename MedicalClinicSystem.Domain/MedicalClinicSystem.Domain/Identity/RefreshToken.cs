namespace MedicalClinicSystem.Domain.Entities.Identity
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsUsable => IsActive && !IsDeleted && RevokedAt == null && !IsExpired;

        public User User { get; set; } = null!;
    }
}
