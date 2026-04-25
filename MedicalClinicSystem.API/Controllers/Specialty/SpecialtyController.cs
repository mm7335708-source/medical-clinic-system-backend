using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Specialty;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminOrReceptionist)]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        [HttpPost]
        public async Task<ActionResult<SpecialtyResponseDto>> Create([FromBody] CreateSpecialtyRequestDto dto)
        {
            var result = await _specialtyService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<SpecialtyResponseDto>>> GetAll()
        {
            var result = await _specialtyService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SpecialtyResponseDto>> GetById(Guid id)
        {
            var result = await _specialtyService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSpecialtyRequestDto dto)
        {
            await _specialtyService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _specialtyService.DeleteAsync(id);
            return NoContent();
        }
    }
}
