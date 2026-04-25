using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task LogoutAsync(LogoutRequestDto request);
    }
}
