using MedicalClinicSystem.Application.DTOs.Common;

namespace MedicalClinicSystem.Application.DTOs.Doctor
{
    public class DoctorFilterRequestDto : PaginationRequestDto
    {
        public Guid? ClinicId { get; set; }
        public Guid? SpecialtyId { get; set; }
        public string? Name { get; set; }
    }
}