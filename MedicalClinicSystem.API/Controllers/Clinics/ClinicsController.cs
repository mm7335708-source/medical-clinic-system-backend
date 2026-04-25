using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Clinic;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AllStaff)]
    public class ClinicsController : ControllerBase
    {
        private readonly IClinicService _clinicService;

        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPost]
        public async Task<ActionResult<ClinicResponseDto>> Create([FromBody] CreateClinicRequestDto dto)
        {
            var result = await _clinicService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<ClinicResponseDto>>> GetAll()
        {
            var result = await _clinicService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClinicResponseDto>> GetById(Guid id)
        {
            var result = await _clinicService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClinicRequestDto dto)
        {
            await _clinicService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.AdminOrReceptionist)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _clinicService.DeleteAsync(id);
            return NoContent();
        }
    }
}
