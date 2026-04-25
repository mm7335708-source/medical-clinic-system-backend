
namespace MedicalClinicSystem.Domain.Entities
{
    public class Specialty : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Navigation Properties
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
