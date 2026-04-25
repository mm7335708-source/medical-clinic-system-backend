using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleResponseDto>> GetAllAsync();
        Task<RoleResponseDto> GetByIdAsync(Guid id);
    }
}
