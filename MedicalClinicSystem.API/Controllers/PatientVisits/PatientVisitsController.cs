using MedicalClinicSystem.API.Authorization;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.PatientVisit;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalClinicSystem.API.Controllers.PatientVisits
{
    [ApiController]
    [Route("api/patient-visits")]
    [Authorize(Roles = AppRoles.AllStaff)]
    public class PatientVisitsController : ControllerBase
    {
        private readonly IPatientVisitService _patientVisitService;

        public PatientVisitsController(IPatientVisitService patientVisitService)
        {
            _patientVisitService = patientVisitService;
        }

        [Authorize(Roles = AppRoles.AdminOrDoctor)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePatientVisitRequestDto request)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();
                if (request.DoctorId != doctorId.Value) return Forbid();
            }

            var result = await _patientVisitService.CreateAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrDoctor)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientVisitRequestDto request)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();
                if (request.DoctorId != doctorId.Value) return Forbid();
            }

            var result = await _patientVisitService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _patientVisitService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("by-patient/{patientId:guid}")]
        public async Task<IActionResult> GetByPatientId(Guid patientId)
        {
            var result = await _patientVisitService.GetByPatientIdAsync(patientId);
            return Ok(result);
        }

        [HttpGet("by-doctor/{doctorId:guid}")]
        public async Task<IActionResult> GetByDoctorId(Guid doctorId)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var myDoctorId = GetCurrentDoctorIdOrForbid();
                if (myDoctorId == null) return Forbid();
                if (myDoctorId.Value != doctorId) return Forbid();
            }

            var result = await _patientVisitService.GetByDoctorIdAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("by-clinic/{clinicId:guid}")]
        public async Task<IActionResult> GetByClinicId(Guid clinicId)
        {
            var result = await _patientVisitService.GetByClinicIdAsync(clinicId);
            return Ok(result);
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var result = await _patientVisitService.GetByDateRangeAsync(fromDate, toDate);
            return Ok(result);
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayVisits()
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();

                var filter = new PatientVisitFilterRequestDto
                {
                    DoctorId = doctorId.Value,
                    FromDate = DateTime.UtcNow.Date,
                    ToDate = DateTime.UtcNow.Date,
                    PageNumber = 1,
                    PageSize = 500
                };

                var paged = await _patientVisitService.GetFilteredPagedAsync(filter);
                return Ok(paged.Items);
            }

            var result = await _patientVisitService.GetTodayVisitsAsync();
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto request)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();

                var filter = new PatientVisitFilterRequestDto
                {
                    DoctorId = doctorId.Value,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var resultForDoctor = await _patientVisitService.GetFilteredPagedAsync(filter);
                return Ok(resultForDoctor);
            }

            var result = await _patientVisitService.GetPagedAsync(request);
            return Ok(result);
        }

        [HttpGet("filtered-paged")]
        public async Task<IActionResult> GetFilteredPaged([FromQuery] PatientVisitFilterRequestDto request)
        {
            if (User.IsInRole(AppRoles.Doctor))
            {
                var doctorId = GetCurrentDoctorIdOrForbid();
                if (doctorId == null) return Forbid();
                request.DoctorId = doctorId.Value;
            }

            var result = await _patientVisitService.GetFilteredPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrDoctor)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _patientVisitService.DeleteAsync(id);
            return Ok(new { message = "Patient visit deleted successfully." });
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
