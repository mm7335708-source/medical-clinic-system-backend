using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Doctor;
using MedicalClinicSystem.Application.DTOs.DoctorSchedule;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorResponseDto> CreateAsync(CreateDoctorRequestDto dto);
        Task<List<DoctorResponseDto>> GetAllAsync();
        Task<DoctorResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateDoctorRequestDto dto);
        Task DeleteAsync(Guid id);
        Task<PagedResultDto<DoctorResponseDto>> GetPagedAsync(PaginationRequestDto request);
        Task<List<DoctorResponseDto>> GetByClinicAsync(Guid clinicId);
        Task<PagedResultDto<DoctorResponseDto>> GetFilteredPagedAsync(DoctorFilterRequestDto request);
        Task<List<DoctorResponseDto>> GetBySpecialtyAsync(Guid specialtyId);
    }
}