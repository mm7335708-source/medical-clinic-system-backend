using MedicalClinicSystem.Application.DTOs.Appointment;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Domain.Enums;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CreateAsync(CreateAppointmentRequestDto dto);
        Task<List<AppointmentResponseDto>> GetAllAsync();
        Task UpdateAsync(Guid id, UpdateAppointmentRequestDto dto);
        Task<AppointmentResponseDto> GetByIdAsync(Guid id);
        Task UpdateStatusAsync(Guid id, UpdateAppointmentStatusRequestDto dto);
        Task DeleteAsync(Guid id);
        Task<PagedResultDto<AppointmentResponseDto>> GetFilteredPagedAsync(AppointmentFilterRequestDto request);
        Task<PagedResultDto<AppointmentResponseDto>> GetPagedAsync(PaginationRequestDto request);
        Task<List<AppointmentResponseDto>> GetByDateAsync(DateTime date);
        Task<List<AppointmentResponseDto>> GetByDoctorAsync(Guid doctorId);
        Task<List<AppointmentResponseDto>> GetByStatusAsync(AppointmentStatus status);
        Task CancelAsync(Guid id, CancelAppointmentRequestDto dto);
        Task<List<AvailableSlotResponseDto>> GetAvailableSlotsAsync(Guid doctorId, DateTime date);
    }
}