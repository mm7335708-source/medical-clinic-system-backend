using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Doctor;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("by-specialty/{specialtyId:guid}")]
        public async Task<ActionResult<List<DoctorResponseDto>>> GetBySpecialty(Guid specialtyId)
        {
            var result = await _doctorService.GetBySpecialtyAsync(specialtyId);
            return Ok(result);
        }

        [HttpGet("filtered-paged")]
        public async Task<ActionResult<PagedResultDto<DoctorResponseDto>>> GetFilteredPaged([FromQuery] DoctorFilterRequestDto request)
        {
            var result = await _doctorService.GetFilteredPagedAsync(request);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<DoctorResponseDto>>> GetPaged([FromQuery] PaginationRequestDto request)
        {
            var result = await _doctorService.GetPagedAsync(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorResponseDto>> Create([FromBody] CreateDoctorRequestDto dto)
        {
            var result = await _doctorService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("by-clinic/{clinicId:guid}")]
        public async Task<ActionResult<List<DoctorResponseDto>>> GetByClinic(Guid clinicId)
        {
            var result = await _doctorService.GetByClinicAsync(clinicId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<DoctorResponseDto>>> GetAll()
        {
            var result = await _doctorService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DoctorResponseDto>> GetById(Guid id)
        {
            var result = await _doctorService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorRequestDto dto)
        {
            await _doctorService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _doctorService.DeleteAsync(id);
            return NoContent();
        }
    }
}
