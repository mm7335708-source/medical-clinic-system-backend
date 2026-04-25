using MedicalClinicSystem.Application.DTOs.Specialty;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface ISpecialtyService
    {
        Task<SpecialtyResponseDto> CreateAsync(CreateSpecialtyRequestDto dto);
        Task<List<SpecialtyResponseDto>> GetAllAsync();
        Task<SpecialtyResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateSpecialtyRequestDto dto);
        Task DeleteAsync(Guid id);
    }
}