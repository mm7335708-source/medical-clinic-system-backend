using MedicalClinicSystem.Application.DTOs.Common;

namespace MedicalClinicSystem.Application.DTOs.PatientVisit
{
    public class PatientVisitFilterRequestDto : PaginationRequestDto
    {
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? ClinicId { get; set; }
        public Guid? AppointmentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
    }
}