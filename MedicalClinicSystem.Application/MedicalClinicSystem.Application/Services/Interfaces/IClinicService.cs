using MedicalClinicSystem.Application.DTOs.Clinic;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IClinicService
    {
        Task<ClinicResponseDto> CreateAsync(CreateClinicRequestDto dto);
        Task<List<ClinicResponseDto>> GetAllAsync();
        Task<ClinicResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateClinicRequestDto dto);
        Task DeleteAsync(Guid id);
    }
}