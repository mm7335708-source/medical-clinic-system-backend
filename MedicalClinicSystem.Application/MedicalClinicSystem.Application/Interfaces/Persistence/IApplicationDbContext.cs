using MedicalClinicSystem.Domain.Entities;
using MedicalClinicSystem.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalClinicSystem.Application.Interfaces.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<Clinic> Clinics { get; }
        DbSet<Doctor> Doctors { get; }
        DbSet<Patient> Patients { get; }
        DbSet<Specialty> Specialties { get; }
        DbSet<DoctorSchedule> DoctorSchedules { get; }
        DbSet<Appointment> Appointments { get; }
        DbSet<PatientVisit> PatientVisits { get; }
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
