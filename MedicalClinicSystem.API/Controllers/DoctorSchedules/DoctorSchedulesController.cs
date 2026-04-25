using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.DoctorSchedule;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public class DoctorSchedulesController : ControllerBase
    {
        private readonly IDoctorScheduleService _doctorScheduleService;

        public DoctorSchedulesController(IDoctorScheduleService doctorScheduleService)
        {
            _doctorScheduleService = doctorScheduleService;
        }

        [HttpPost]
        public async Task<ActionResult<DoctorScheduleResponseDto>> Create([FromBody] CreateDoctorScheduleRequestDto dto)
        {
            var result = await _doctorScheduleService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("by-doctor/{doctorId:guid}")]
        public async Task<ActionResult<List<DoctorScheduleResponseDto>>> GetByDoctor(Guid doctorId)
        {
            var result = await _doctorScheduleService.GetByDoctorAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("by-doctor/{doctorId:guid}/day/{dayOfWeek}")]
        public async Task<ActionResult<DoctorScheduleResponseDto>> GetByDoctorAndDay(Guid doctorId, DayOfWeek dayOfWeek)
        {
            var result = await _doctorScheduleService.GetByDoctorAndDayAsync(doctorId, dayOfWeek);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<DoctorScheduleResponseDto>>> GetAll()
        {
            var result = await _doctorScheduleService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DoctorScheduleResponseDto>> GetById(Guid id)
        {
            var result = await _doctorScheduleService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorScheduleRequestDto dto)
        {
            await _doctorScheduleService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _doctorScheduleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
