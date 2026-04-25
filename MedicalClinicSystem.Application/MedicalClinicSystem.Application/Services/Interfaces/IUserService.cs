using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateAsync(CreateUserRequestDto request);
        Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserRequestDto request);
        Task<UserResponseDto> GetByIdAsync(Guid id);
        Task<UserResponseDto> GetCurrentAsync(Guid id);
        Task<List<UserResponseDto>> GetAllAsync();
        Task<PagedResultDto<UserResponseDto>> GetPagedAsync(PaginationRequestDto request);
        Task<UserResponseDto> UpdateStatusAsync(Guid id, UpdateUserStatusRequestDto request);
        Task DeleteAsync(Guid id);
        Task ChangePasswordAsync(Guid id, ChangePasswordRequestDto request);
        Task ResetPasswordAsync(Guid id, ResetPasswordRequestDto request);
        Task RevokeSessionsAsync(Guid id);
    }
}
