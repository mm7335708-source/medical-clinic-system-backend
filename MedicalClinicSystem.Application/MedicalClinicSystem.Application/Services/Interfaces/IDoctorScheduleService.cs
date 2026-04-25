using MedicalClinicSystem.Application.DTOs.DoctorSchedule;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IDoctorScheduleService
    {
        Task<DoctorScheduleResponseDto> CreateAsync(CreateDoctorScheduleRequestDto dto);
        Task<List<DoctorScheduleResponseDto>> GetAllAsync();
        Task<DoctorScheduleResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateDoctorScheduleRequestDto dto);
        Task DeleteAsync(Guid id);

        Task<List<DoctorScheduleResponseDto>> GetByDoctorAsync(Guid doctorId);
        Task<DoctorScheduleResponseDto> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek dayOfWeek);

    }
}