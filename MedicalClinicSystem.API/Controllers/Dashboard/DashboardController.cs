using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Dashboard;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AllStaff)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponseDto<DashboardSummaryResponseDto>>> GetSummary()
        {
            var result = await _dashboardService.GetSummaryAsync();

            return Ok(new ApiResponseDto<DashboardSummaryResponseDto>
            {
                Success = true,
                Message = "Dashboard summary retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("today-appointments")]
        public async Task<ActionResult<ApiResponseDto<List<TodayAppointmentResponseDto>>>> GetTodayAppointments()
        {
            var result = await _dashboardService.GetTodayAppointmentsAsync();

            return Ok(new ApiResponseDto<List<TodayAppointmentResponseDto>>
            {
                Success = true,
                Message = "Today's appointments retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("today-appointments-by-status")]
        public async Task<ActionResult<ApiResponseDto<List<TodayAppointmentsByStatusResponseDto>>>> GetTodayAppointmentsByStatus()
        {
            var result = await _dashboardService.GetTodayAppointmentsByStatusAsync();

            return Ok(new ApiResponseDto<List<TodayAppointmentsByStatusResponseDto>>
            {
                Success = true,
                Message = "Today's appointment statistics retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("doctor-summary/{doctorId:guid}")]
        public async Task<ActionResult<ApiResponseDto<DoctorSummaryResponseDto>>> GetDoctorSummary(Guid doctorId)
        {
            var result = await _dashboardService.GetDoctorSummaryAsync(doctorId);

            return Ok(new ApiResponseDto<DoctorSummaryResponseDto>
            {
                Success = true,
                Message = "Doctor summary retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("upcoming-appointments")]
        public async Task<ActionResult<ApiResponseDto<List<UpcomingAppointmentResponseDto>>>> GetUpcomingAppointments([FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetUpcomingAppointmentsAsync(count);

            return Ok(new ApiResponseDto<List<UpcomingAppointmentResponseDto>>
            {
                Success = true,
                Message = "Upcoming appointments retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("busy-doctors-today")]
        public async Task<ActionResult<ApiResponseDto<List<BusyDoctorTodayResponseDto>>>> GetBusyDoctorsToday([FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetBusyDoctorsTodayAsync(count);

            return Ok(new ApiResponseDto<List<BusyDoctorTodayResponseDto>>
            {
                Success = true,
                Message = "Busy doctors retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("daily-performance")]
        public async Task<ActionResult<ApiResponseDto<DailyPerformanceResponseDto>>> GetDailyPerformance()
        {
            var result = await _dashboardService.GetDailyPerformanceAsync();

            return Ok(new ApiResponseDto<DailyPerformanceResponseDto>
            {
                Success = true,
                Message = "Daily performance retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("today-visits")]
        public async Task<ActionResult<ApiResponseDto<List<TodayPatientVisitResponseDto>>>> GetTodayVisits()
        {
            var result = await _dashboardService.GetTodayVisitsAsync();

            return Ok(new ApiResponseDto<List<TodayPatientVisitResponseDto>>
            {
                Success = true,
                Message = "Today's visits retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("clinics-activity")]
        public async Task<ActionResult<ApiResponseDto<List<ClinicActivityResponseDto>>>> GetClinicsActivity()
        {
            var result = await _dashboardService.GetClinicsActivityAsync();

            return Ok(new ApiResponseDto<List<ClinicActivityResponseDto>>
            {
                Success = true,
                Message = "Clinic activity retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("doctor-visits/{doctorId:guid}")]
        public async Task<ActionResult<ApiResponseDto<DoctorVisitsSummaryResponseDto>>> GetDoctorVisitsSummary(Guid doctorId)
        {
            var result = await _dashboardService.GetDoctorVisitsSummaryAsync(doctorId);

            return Ok(new ApiResponseDto<DoctorVisitsSummaryResponseDto>
            {
                Success = true,
                Message = "Doctor visits summary retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("clinic-visits/{clinicId:guid}")]
        public async Task<ActionResult<ApiResponseDto<ClinicVisitsSummaryResponseDto>>> GetClinicVisitsSummary(Guid clinicId)
        {
            var result = await _dashboardService.GetClinicVisitsSummaryAsync(clinicId);

            return Ok(new ApiResponseDto<ClinicVisitsSummaryResponseDto>
            {
                Success = true,
                Message = "Clinic visits summary retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("visits-summary")]
        public async Task<ActionResult<ApiResponseDto<VisitsSummaryResponseDto>>> GetVisitsSummary()
        {
            var result = await _dashboardService.GetVisitsSummaryAsync();

            return Ok(new ApiResponseDto<VisitsSummaryResponseDto>
            {
                Success = true,
                Message = "Visits summary retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("appointments-vs-visits")]
        public async Task<ActionResult<ApiResponseDto<AppointmentsVsVisitsResponseDto>>> GetAppointmentsVsVisits()
        {
            var result = await _dashboardService.GetAppointmentsVsVisitsAsync();

            return Ok(new ApiResponseDto<AppointmentsVsVisitsResponseDto>
            {
                Success = true,
                Message = "Appointments versus visits data retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("busy-clinics-today")]
        public async Task<ActionResult<ApiResponseDto<List<BusyClinicTodayResponseDto>>>> GetBusyClinicsToday([FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetBusyClinicsTodayAsync(count);

            return Ok(new ApiResponseDto<List<BusyClinicTodayResponseDto>>
            {
                Success = true,
                Message = "Busy clinics retrieved successfully.",
                Data = result
            });
        }
    }
}
