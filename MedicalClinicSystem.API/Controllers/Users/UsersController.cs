using System.Security.Claims;
using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Identity;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinicSystem.API.Controllers.Users
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserRequestDto request)
        {
            var result = await _userService.CreateAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserResponseDto>> Update(Guid id, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _userService.UpdateAsync(id, request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult<UserResponseDto>> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequestDto request)
        {
            var result = await _userService.UpdateStatusAsync(id, request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpPut("{id:guid}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequestDto request)
        {
            await _userService.ResetPasswordAsync(id, request);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpPost("{id:guid}/revoke-sessions")]
        public async Task<IActionResult> RevokeSessions(Guid id)
        {
            await _userService.RevokeSessionsAsync(id);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.AllStaff)]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponseDto>> GetCurrent()
        {
            var userId = GetCurrentUserId();
            var result = await _userService.GetCurrentAsync(userId);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AllStaff)]
        [HttpPut("me/change-password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordRequestDto request)
        {
            var userId = GetCurrentUserId();
            await _userService.ChangePasswordAsync(userId, request);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<UserResponseDto>>> GetPaged([FromQuery] PaginationRequestDto request)
        {
            var result = await _userService.GetPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOnly)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedException("Unable to determine the current user from the token.");
            }

            return userId;
        }
    }
}
