using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Appointment;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AllStaff)]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<AppointmentResponseDto>>> GetPaged([FromQuery] PaginationRequestDto request)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();

                var filtered = new AppointmentFilterRequestDto
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    DoctorId = doctorId.Value
                };

                var resultForDoctor = await _appointmentService.GetFilteredPagedAsync(filtered);
                return Ok(resultForDoctor);
            }

            var result = await _appointmentService.GetPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentRequestDto dto)
        {
            await _appointmentService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpGet("filtered-paged")]
        public async Task<ActionResult<PagedResultDto<AppointmentResponseDto>>> GetFilteredPaged([FromQuery] AppointmentFilterRequestDto request)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();
                request.DoctorId = doctorId.Value;
            }

            var result = await _appointmentService.GetFilteredPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentRequestDto dto)
        {
            await _appointmentService.CancelAsync(id, dto);
            return NoContent();
        }

        [HttpGet("available-slots")]
        public async Task<ActionResult<List<AvailableSlotResponseDto>>> GetAvailableSlots([FromQuery] Guid doctorId, [FromQuery] DateTime date)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var myDoctorId = GetCurrentDoctorIdOrForbid();
                if (myDoctorId == null) return Forbid();
                if (myDoctorId.Value != doctorId) return Forbid();
            }

            var result = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPost]
        public async Task<ActionResult<AppointmentResponseDto>> Create([FromBody] CreateAppointmentRequestDto dto)
        {
            var result = await _appointmentService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetAll()
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();

                var resultForDoctor = await _appointmentService.GetByDoctorAsync(doctorId.Value);
                return Ok(resultForDoctor);
            }

            var result = await _appointmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("by-status")]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetByStatus([FromQuery] AppointmentStatus status)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();

                var mine = await _appointmentService.GetByDoctorAsync(doctorId.Value);
                return Ok(mine.Where(x => x.Status == status).ToList());
            }

            var result = await _appointmentService.GetByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("by-doctor/{doctorId:guid}")]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetByDoctor(Guid doctorId)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var myDoctorId = GetCurrentDoctorIdOrForbid();
                if (myDoctorId == null) return Forbid();
                if (myDoctorId.Value != doctorId) return Forbid();
            }

            var result = await _appointmentService.GetByDoctorAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AppointmentResponseDto>> GetById(Guid id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("by-date")]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetByDate([FromQuery] DateTime date)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();

                var mine = await _appointmentService.GetByDoctorAsync(doctorId.Value);
                var targetDate = date.Date;
                return Ok(mine.Where(x => x.AppointmentDate.Date == targetDate).ToList());
            }

            var result = await _appointmentService.GetByDateAsync(date);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusRequestDto dto)
        {
            await _appointmentService.UpdateStatusAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }

        private Guid? GetCurrentDoctorIdOrForbid()
        {
            var doctorIdValue = User.FindFirstValue("DoctorId");
            if (!Guid.TryParse(doctorIdValue, out var doctorId))
            {
                return null;
            }

            return doctorId;
        }
    }
}
