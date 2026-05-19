using MedicalClinicSystem.Application.DTOs.Dashboard;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IApplicationDbContext context,
            ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardSummaryResponseDto> GetSummaryAsync()
        {
            var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            var tomorrow = today.AddDays(1);
            var weekStart = DateTime.SpecifyKind(today.AddDays(-(int)today.DayOfWeek), DateTimeKind.Utc);
            var monthStart = DateTime.SpecifyKind(new DateTime(today.Year, today.Month, 1), DateTimeKind.Utc);

            var totalClinics = await _context.Clinics.CountAsync(x => !x.IsDeleted);
            var totalDoctors = await _context.Doctors.CountAsync(x => !x.IsDeleted);
            var totalPatients = await _context.Patients.CountAsync(x => !x.IsDeleted);
            var totalAppointments = await _context.Appointments.CountAsync(x => !x.IsDeleted);

            var todayAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.AppointmentDate.Date == today);

            var pendingAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.Status == AppointmentStatus.Pending);

            var confirmedAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.Status == AppointmentStatus.Confirmed);

            var cancelledAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.Status == AppointmentStatus.Cancelled);

            var completedAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.Status == AppointmentStatus.Completed);

            var totalVisits = await _context.PatientVisits.CountAsync();

            var todayVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate >= today && x.VisitDate < tomorrow);

            var thisWeekVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate.Date >= weekStart);

            var thisMonthVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate.Date >= monthStart);

            _logger.LogInformation("Dashboard summary retrieved successfully.");

            return new DashboardSummaryResponseDto
            {
                TotalClinics = totalClinics,
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,

                TotalAppointments = totalAppointments,
                TodayAppointments = todayAppointments,
                PendingAppointments = pendingAppointments,
                ConfirmedAppointments = confirmedAppointments,
                CancelledAppointments = cancelledAppointments,
                CompletedAppointments = completedAppointments,

                TotalVisits = totalVisits,
                TodayVisits = todayVisits,
                ThisWeekVisits = thisWeekVisits,
                ThisMonthVisits = thisMonthVisits
            };
        }

        public async Task<List<TodayAppointmentResponseDto>> GetTodayAppointmentsAsync()
        {
            var today = DateTime.UtcNow.Date;

            var appointments = await _context.Appointments
                .Where(x => !x.IsDeleted && x.AppointmentDate.Date == today)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .OrderBy(x => x.StartTime)
                .ToListAsync();

            return appointments.Select(x => new TodayAppointmentResponseDto
            {
                AppointmentId = x.Id,
                DoctorName = x.Doctor != null ? x.Doctor.FullName : "N/A",
                PatientName = x.Patient != null ? x.Patient.FullName : "N/A",
                ClinicName = x.Clinic != null ? x.Clinic.ClinicName : "N/A",
                StartTime = x.StartTime.ToString(),
                Status = x.Status,
                Notes = x.Notes
            }).ToList();
        }

        public async Task<List<TodayAppointmentsByStatusResponseDto>> GetTodayAppointmentsByStatusAsync()
        {
            var today = DateTime.UtcNow.Date;

            var grouped = await _context.Appointments
                .Where(x => !x.IsDeleted && x.AppointmentDate.Date == today)
                .GroupBy(x => x.Status)
                .Select(g => new TodayAppointmentsByStatusResponseDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToListAsync();

            return grouped;
        }

        public async Task<DoctorSummaryResponseDto> GetDoctorSummaryAsync(Guid doctorId)
        {
            var today = DateTime.UtcNow.Date;

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(x => x.Id == doctorId && !x.IsDeleted);

            if (doctor == null)
                throw new NotFoundException("الطبيب غير موجود.");

            var totalAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.DoctorId == doctorId);

            var todayAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.DoctorId == doctorId && x.AppointmentDate.Date == today);

            var pendingAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.DoctorId == doctorId && x.Status == AppointmentStatus.Pending);

            var confirmedAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.DoctorId == doctorId && x.Status == AppointmentStatus.Confirmed);

            var cancelledAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.DoctorId == doctorId && x.Status == AppointmentStatus.Cancelled);

            var completedAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.DoctorId == doctorId && x.Status == AppointmentStatus.Completed);

            _logger.LogInformation("Doctor dashboard summary retrieved successfully. DoctorId: {DoctorId}", doctorId);

            return new DoctorSummaryResponseDto
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.FullName,
                TotalAppointments = totalAppointments,
                TodayAppointments = todayAppointments,
                PendingAppointments = pendingAppointments,
                ConfirmedAppointments = confirmedAppointments,
                CancelledAppointments = cancelledAppointments,
                CompletedAppointments = completedAppointments
            };
        }

        public async Task<List<UpcomingAppointmentResponseDto>> GetUpcomingAppointmentsAsync(int count = 5)
        {
            var today = DateTime.UtcNow.Date;
            var nowTime = DateTime.UtcNow.TimeOfDay;

            var appointments = await _context.Appointments
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status != AppointmentStatus.Cancelled &&
                    (
                        x.AppointmentDate.Date > today ||
                        (x.AppointmentDate.Date == today && x.StartTime >= nowTime)
                    ))
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .OrderBy(x => x.AppointmentDate)
                .ThenBy(x => x.StartTime)
                .Take(count)
                .ToListAsync();

            return appointments.Select(x => new UpcomingAppointmentResponseDto
            {
                AppointmentId = x.Id,
                AppointmentDate = x.AppointmentDate,
                StartTime = x.StartTime.ToString(),
                DoctorName = x.Doctor != null ? x.Doctor.FullName : "N/A",
                PatientName = x.Patient != null ? x.Patient.FullName : "N/A",
                ClinicName = x.Clinic != null ? x.Clinic.ClinicName : "N/A",
                Status = x.Status
            }).ToList();
        }

        public async Task<List<BusyDoctorTodayResponseDto>> GetBusyDoctorsTodayAsync(int count = 5)
        {
            var today = DateTime.UtcNow.Date;

            var result = await _context.Appointments
                .Where(x => !x.IsDeleted && x.AppointmentDate.Date == today)
                .Include(x => x.Doctor)
                .GroupBy(x => new { x.DoctorId, x.Doctor.FullName })
                .Select(g => new BusyDoctorTodayResponseDto
                {
                    DoctorId = g.Key.DoctorId,
                    DoctorName = g.Key.FullName,
                    AppointmentsCount = g.Count()
                })
                .OrderByDescending(x => x.AppointmentsCount)
                .ThenBy(x => x.DoctorName)
                .Take(count)
                .ToListAsync();

            return result;
        }

        public async Task<List<ClinicActivityResponseDto>> GetClinicsActivityAsync()
        {
            var result = await _context.Appointments
                .Where(x => !x.IsDeleted)
                .Include(x => x.Clinic)
                .GroupBy(x => new { x.ClinicId, x.Clinic.ClinicName })
                .Select(g => new ClinicActivityResponseDto
                {
                    ClinicId = g.Key.ClinicId,
                    ClinicName = g.Key.ClinicName,
                    AppointmentsCount = g.Count()
                })
                .OrderByDescending(x => x.AppointmentsCount)
                .ThenBy(x => x.ClinicName)
                .ToListAsync();

            return result;
        }

        public async Task<List<TodayPatientVisitResponseDto>> GetTodayVisitsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var visits = await _context.PatientVisits
                .Where(x => x.VisitDate >= today && x.VisitDate < tomorrow)
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.Clinic)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();

            return visits.Select(x => new TodayPatientVisitResponseDto
            {
                VisitId = x.Id,
                PatientId = x.PatientId,
                PatientName = x.Patient != null ? x.Patient.FullName : "غير متوفر",
                DoctorId = x.DoctorId,
                DoctorName = x.Doctor != null ? x.Doctor.FullName : "غير متوفر",
                ClinicId = x.ClinicId,
                ClinicName = x.Clinic != null ? x.Clinic.ClinicName : "غير متوفر",
                VisitDate = x.VisitDate,
                ChiefComplaint = x.ChiefComplaint,
                Diagnosis = x.Diagnosis,
                Notes = x.Notes
            }).ToList();
        }

        public async Task<DoctorVisitsSummaryResponseDto> GetDoctorVisitsSummaryAsync(Guid doctorId)
        {
            var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            var weekStart = DateTime.SpecifyKind(today.AddDays(-(int)today.DayOfWeek), DateTimeKind.Utc);
            var monthStart = DateTime.SpecifyKind(new DateTime(today.Year, today.Month, 1), DateTimeKind.Utc);

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(x => x.Id == doctorId && !x.IsDeleted);

            if (doctor == null)
                throw new NotFoundException("الطبيب غير موجود.");

            var totalVisits = await _context.PatientVisits
                .CountAsync(x => x.DoctorId == doctorId);

            var todayVisits = await _context.PatientVisits
                .CountAsync(x => x.DoctorId == doctorId && x.VisitDate.Date == today);

            var thisWeekVisits = await _context.PatientVisits
                .CountAsync(x => x.DoctorId == doctorId && x.VisitDate.Date >= weekStart);

            var thisMonthVisits = await _context.PatientVisits
                .CountAsync(x => x.DoctorId == doctorId && x.VisitDate.Date >= monthStart);

            return new DoctorVisitsSummaryResponseDto
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.FullName,
                TotalVisits = totalVisits,
                TodayVisits = todayVisits,
                ThisWeekVisits = thisWeekVisits,
                ThisMonthVisits = thisMonthVisits
            };
        }

        public async Task<ClinicVisitsSummaryResponseDto> GetClinicVisitsSummaryAsync(Guid clinicId)
        {
            var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            var weekStart = DateTime.SpecifyKind(today.AddDays(-(int)today.DayOfWeek), DateTimeKind.Utc);
            var monthStart = DateTime.SpecifyKind(new DateTime(today.Year, today.Month, 1), DateTimeKind.Utc);

            var clinic = await _context.Clinics
                .FirstOrDefaultAsync(x => x.Id == clinicId && !x.IsDeleted);

            if (clinic == null)
                throw new NotFoundException("العيادة غير موجودة.");

            var totalVisits = await _context.PatientVisits
                .CountAsync(x => x.ClinicId == clinicId);

            var todayVisits = await _context.PatientVisits
                .CountAsync(x => x.ClinicId == clinicId && x.VisitDate.Date == today);

            var thisWeekVisits = await _context.PatientVisits
                .CountAsync(x => x.ClinicId == clinicId && x.VisitDate.Date >= weekStart);

            var thisMonthVisits = await _context.PatientVisits
                .CountAsync(x => x.ClinicId == clinicId && x.VisitDate.Date >= monthStart);

            return new ClinicVisitsSummaryResponseDto
            {
                ClinicId = clinic.Id,
                ClinicName = clinic.ClinicName,
                TotalVisits = totalVisits,
                TodayVisits = todayVisits,
                ThisWeekVisits = thisWeekVisits,
                ThisMonthVisits = thisMonthVisits
            };
        }

        public async Task<VisitsSummaryResponseDto> GetVisitsSummaryAsync()
        {
            var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            var weekStart = DateTime.SpecifyKind(today.AddDays(-(int)today.DayOfWeek), DateTimeKind.Utc);
            var monthStart = DateTime.SpecifyKind(new DateTime(today.Year, today.Month, 1), DateTimeKind.Utc);

            var totalVisits = await _context.PatientVisits.CountAsync();

            var todayVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate.Date == today);

            var thisWeekVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate.Date >= weekStart);

            var thisMonthVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate.Date >= monthStart);

            return new VisitsSummaryResponseDto
            {
                TotalVisits = totalVisits,
                TodayVisits = todayVisits,
                ThisWeekVisits = thisWeekVisits,
                ThisMonthVisits = thisMonthVisits
            };
        }

        public async Task<AppointmentsVsVisitsResponseDto> GetAppointmentsVsVisitsAsync()
        {
            var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);

            var totalAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted);

            var totalVisits = await _context.PatientVisits
                .CountAsync();

            var todayAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.AppointmentDate.Date == today);

            var todayVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate.Date == today);

            var completedAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.Status == AppointmentStatus.Completed);

            var cancelledAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.Status == AppointmentStatus.Cancelled);

            return new AppointmentsVsVisitsResponseDto
            {
                TotalAppointments = totalAppointments,
                TotalVisits = totalVisits,
                TodayAppointments = todayAppointments,
                TodayVisits = todayVisits,
                CompletedAppointments = completedAppointments,
                CancelledAppointments = cancelledAppointments
            };
        }

        public async Task<List<BusyClinicTodayResponseDto>> GetBusyClinicsTodayAsync(int count = 5)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var result = await _context.PatientVisits
                .Where(x => x.VisitDate >= today && x.VisitDate < tomorrow)
                .Include(x => x.Clinic)
                .GroupBy(x => new { x.ClinicId, x.Clinic.ClinicName })
                .Select(g => new BusyClinicTodayResponseDto
                {
                    ClinicId = g.Key.ClinicId,
                    ClinicName = g.Key.ClinicName,
                    VisitsCount = g.Count()
                })
                .OrderByDescending(x => x.VisitsCount)
                .ThenBy(x => x.ClinicName)
                .Take(count)
                .ToListAsync();

            return result;
        }

        public async Task<DailyPerformanceResponseDto> GetDailyPerformanceAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var todayAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted && x.AppointmentDate.Date == today);

            var completedTodayAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted &&
                                 x.AppointmentDate.Date == today &&
                                 x.Status == AppointmentStatus.Completed);

            var cancelledTodayAppointments = await _context.Appointments
                .CountAsync(x => !x.IsDeleted &&
                                 x.AppointmentDate.Date == today &&
                                 x.Status == AppointmentStatus.Cancelled);

            var todayVisits = await _context.PatientVisits
                .CountAsync(x => x.VisitDate >= today && x.VisitDate < tomorrow);

            var unattendedAppointments = todayAppointments - completedTodayAppointments - cancelledTodayAppointments;
            if (unattendedAppointments < 0)
                unattendedAppointments = 0;

            decimal completionRate = 0;
            decimal visitConversionRate = 0;

            if (todayAppointments > 0)
            {
                completionRate = Math.Round((decimal)completedTodayAppointments / todayAppointments * 100, 2);
                visitConversionRate = Math.Round((decimal)todayVisits / todayAppointments * 100, 2);
            }

            return new DailyPerformanceResponseDto
            {
                TodayAppointments = todayAppointments,
                CompletedTodayAppointments = completedTodayAppointments,
                CancelledTodayAppointments = cancelledTodayAppointments,
                TodayVisits = todayVisits,
                UnattendedAppointments = unattendedAppointments,
                CompletionRate = completionRate,
                VisitConversionRate = visitConversionRate
            };
        }
    }
}
