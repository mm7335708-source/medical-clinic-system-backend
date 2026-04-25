using MedicalClinicSystem.Application.DTOs.Dashboard;

namespace MedicalClinicSystem.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponseDto> GetSummaryAsync();
        Task<List<TodayAppointmentResponseDto>> GetTodayAppointmentsAsync();
        Task<List<TodayAppointmentsByStatusResponseDto>> GetTodayAppointmentsByStatusAsync();
        Task<DoctorSummaryResponseDto> GetDoctorSummaryAsync(Guid doctorId);
        Task<List<UpcomingAppointmentResponseDto>> GetUpcomingAppointmentsAsync(int count = 5);
        Task<List<BusyDoctorTodayResponseDto>> GetBusyDoctorsTodayAsync(int count = 5);
        Task<List<ClinicActivityResponseDto>> GetClinicsActivityAsync();
        Task<List<TodayPatientVisitResponseDto>> GetTodayVisitsAsync();
        Task<DoctorVisitsSummaryResponseDto> GetDoctorVisitsSummaryAsync(Guid doctorId);
        Task<ClinicVisitsSummaryResponseDto> GetClinicVisitsSummaryAsync(Guid clinicId);
        Task<VisitsSummaryResponseDto> GetVisitsSummaryAsync();
        Task<AppointmentsVsVisitsResponseDto> GetAppointmentsVsVisitsAsync();
        Task<List<BusyClinicTodayResponseDto>> GetBusyClinicsTodayAsync(int count = 5);
        Task<DailyPerformanceResponseDto> GetDailyPerformanceAsync();
    }
}