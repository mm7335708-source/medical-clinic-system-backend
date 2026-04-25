using MedicalClinicSystem.Domain.Entities.Identity;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, string roleName);
        DateTime GetAccessTokenExpiration();
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
        DateTime GetRefreshTokenExpiration();
    }
}
