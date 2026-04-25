using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Patient;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AllStaff)]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPost]
        public async Task<ActionResult<PatientResponseDto>> Create([FromBody] CreatePatientRequestDto dto)
        {
            var result = await _patientService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<PatientResponseDto>>> GetAll()
        {
            var result = await _patientService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PatientResponseDto>> GetById(Guid id)
        {
            var result = await _patientService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequestDto dto)
        {
            await _patientService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpGet("by-phone/{phoneNumber}")]
        public async Task<ActionResult<PatientResponseDto>> GetByPhone(string phoneNumber)
        {
            var result = await _patientService.GetByPhoneAsync(phoneNumber);
            return Ok(result);
        }

        [HttpGet("filtered-paged")]
        public async Task<ActionResult<PagedResultDto<PatientResponseDto>>> GetFilteredPaged([FromQuery] PatientFilterRequestDto request)
        {
            var result = await _patientService.GetFilteredPagedAsync(request);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<PatientResponseDto>>> GetPaged([FromQuery] PaginationRequestDto request)
        {
            var result = await _patientService.GetPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _patientService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<PatientResponseDto>>> Search([FromQuery] string? name, [FromQuery] string? phone)
        {
            var result = await _patientService.SearchAsync(name, phone);
            return Ok(result);
        }
    }
}
