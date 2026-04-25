using MedicalClinicSystem.Application.DTOs.Common;

namespace MedicalClinicSystem.Application.DTOs.Patient
{
    public class PatientFilterRequestDto : PaginationRequestDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
    }
}