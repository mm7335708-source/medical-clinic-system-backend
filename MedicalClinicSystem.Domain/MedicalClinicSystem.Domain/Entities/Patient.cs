using MedicalClinicSystem.Domain.Enums;


namespace MedicalClinicSystem.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? Address { get; set; }

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<PatientVisit> PatientVisits { get; set; } = new HashSet<PatientVisit>();
    }
}