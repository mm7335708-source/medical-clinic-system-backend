using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Identity;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers.Roles
{
    [ApiController]
    [Route("api/roles")]
    [Authorize(Roles = AppRoles.AdminOnly)]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleResponseDto>>> GetAll()
        {
            var result = await _roleService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoleResponseDto>> GetById(Guid id)
        {
            var result = await _roleService.GetByIdAsync(id);
            return Ok(result);
        }
    }
}
