using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.PatientVisit;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IPatientVisitService
    {
        Task<PatientVisitResponseDto> CreateAsync(CreatePatientVisitRequestDto request);
        Task<PatientVisitResponseDto> UpdateAsync(Guid id, UpdatePatientVisitRequestDto request);
        Task<PatientVisitResponseDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PatientVisitResponseDto>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<PatientVisitResponseDto>> GetByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<PatientVisitResponseDto>> GetByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<PatientVisitResponseDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PatientVisitResponseDto>> GetTodayVisitsAsync();
        Task<PagedResultDto<PatientVisitResponseDto>> GetPagedAsync(PaginationRequestDto request);
        Task<PagedResultDto<PatientVisitResponseDto>> GetFilteredPagedAsync(PatientVisitFilterRequestDto request);
        Task DeleteAsync(Guid id);
    }
}