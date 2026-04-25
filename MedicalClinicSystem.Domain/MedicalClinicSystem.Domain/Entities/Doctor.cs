using MedicalClinicSystem.Domain.Enums;


namespace MedicalClinicSystem.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public virtual ICollection<PatientVisit> PatientVisits { get; set; } = new HashSet<PatientVisit>();
        public int ExperienceYears { get; set; }

        public Guid SpecialtyId { get; set; }

        public Guid ClinicId { get; set; }

        // Navigation Properties
        public Specialty Specialty { get; set; } = null!;

        public DoctorStatus Status { get; set; } = DoctorStatus.Active;
        public Clinic Clinic { get; set; } = null!;

        public ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}