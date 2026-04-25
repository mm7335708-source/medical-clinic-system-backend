using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Patient;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PatientResponseDto> CreateAsync(CreatePatientRequestDto dto);
        Task<List<PatientResponseDto>> GetAllAsync();
        Task<PatientResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdatePatientRequestDto dto);
        Task DeleteAsync(Guid id);
        Task<PatientResponseDto> GetByPhoneAsync(string phoneNumber);
        Task<PagedResultDto<PatientResponseDto>> GetFilteredPagedAsync(PatientFilterRequestDto request);
        Task<PagedResultDto<PatientResponseDto>> GetPagedAsync(PaginationRequestDto request);
        Task<List<PatientResponseDto>> SearchAsync(string? name, string? phone);
    }
}