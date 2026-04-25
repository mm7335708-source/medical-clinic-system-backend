using MedicalClinicSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MedicalClinicSystem.Domain.Entities
{
    public class Clinic : BaseEntity
    {
        public string ClinicName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public virtual ICollection<PatientVisit> PatientVisits { get; set; } = new HashSet<PatientVisit>();
        public string City { get; set; } = string.Empty;

        public ClinicStatus Status { get; set; } = ClinicStatus.Open;
        public string PhoneNumber { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        // Navigation Properties
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}