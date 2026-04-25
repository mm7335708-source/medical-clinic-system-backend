using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.PatientVisit;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using MedicalClinicSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppNotFoundException = MedicalClinicSystem.Application.Exceptions.NotFoundException;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class PatientVisitService : IPatientVisitService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientVisitService> _logger;
        private readonly IValidator<CreatePatientVisitRequestDto> _createValidator;
        private readonly IValidator<UpdatePatientVisitRequestDto> _updateValidator;

        public PatientVisitService(
            IApplicationDbContext context,
            IMapper mapper,
            ILogger<PatientVisitService> logger,
            IValidator<CreatePatientVisitRequestDto> createValidator,
            IValidator<UpdatePatientVisitRequestDto> updateValidator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<PatientVisitResponseDto> CreateAsync(CreatePatientVisitRequestDto request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            await EnsureReferencesExistAsync(request.PatientId, request.DoctorId, request.ClinicId);

            var appointment = await ValidateAppointmentAsync(
                request.AppointmentId,
                request.PatientId,
                request.DoctorId,
                request.ClinicId);

            var entity = _mapper.Map<PatientVisit>(request);

            await _context.PatientVisits.AddAsync(entity);

            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Completed;
                appointment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("تم إنشاء سجل زيارة جديد بنجاح. VisitId: {VisitId}", entity.Id);

            return await GetByIdAsync(entity.Id);
        }

        public async Task<PatientVisitResponseDto> UpdateAsync(Guid id, UpdatePatientVisitRequestDto request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new AppValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var entity = await _context.PatientVisits
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new AppNotFoundException("لم يتم العثور على سجل الزيارة.");

            if (entity.AppointmentId.HasValue)
                throw new AppValidationException(new[] { "لا يمكن تعديل سجل زيارة مرتبط بموعد." });

            await EnsureReferencesExistAsync(request.PatientId, request.DoctorId, request.ClinicId);

            var appointment = await ValidateAppointmentAsync(
                request.AppointmentId,
                request.PatientId,
                request.DoctorId,
                request.ClinicId,
                id);

            entity.PatientId = request.PatientId;
            entity.DoctorId = request.DoctorId;
            entity.ClinicId = request.ClinicId;
            entity.AppointmentId = request.AppointmentId;
            entity.VisitDate = request.VisitDate;
            entity.ChiefComplaint = request.ChiefComplaint;
            entity.Diagnosis = request.Diagnosis;
            entity.TreatmentPlan = request.TreatmentPlan;
            entity.Prescription = request.Prescription;
            entity.Notes = request.Notes;
            entity.FollowUpDate = request.FollowUpDate;

            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Completed;
                appointment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("تم تحديث سجل الزيارة بنجاح. VisitId: {VisitId}", entity.Id);

            return await GetByIdAsync(entity.Id);
        }

        public async Task<PatientVisitResponseDto> GetByIdAsync(Guid id)
        {
            var entity = await BuildQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new AppNotFoundException("لم يتم العثور على سجل الزيارة.");

            return _mapper.Map<PatientVisitResponseDto>(entity);
        }

        public async Task<IEnumerable<PatientVisitResponseDto>> GetByPatientIdAsync(Guid patientId)
        {
            var visits = await BuildQuery()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientVisitResponseDto>>(visits);
        }

        public async Task<IEnumerable<PatientVisitResponseDto>> GetByDoctorIdAsync(Guid doctorId)
        {
            var visits = await BuildQuery()
                .Where(x => x.DoctorId == doctorId)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientVisitResponseDto>>(visits);
        }

        public async Task<IEnumerable<PatientVisitResponseDto>> GetByClinicIdAsync(Guid clinicId)
        {
            var visits = await BuildQuery()
                .Where(x => x.ClinicId == clinicId)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientVisitResponseDto>>(visits);
        }

        public async Task<IEnumerable<PatientVisitResponseDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var from = fromDate.Date;
            var to = toDate.Date.AddDays(1).AddTicks(-1);

            var visits = await BuildQuery()
                .Where(x => x.VisitDate >= from && x.VisitDate <= to)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientVisitResponseDto>>(visits);
        }

        public async Task<IEnumerable<PatientVisitResponseDto>> GetTodayVisitsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var visits = await BuildQuery()
                .Where(x => x.VisitDate >= today && x.VisitDate < tomorrow)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientVisitResponseDto>>(visits);
        }

        public async Task<PagedResultDto<PatientVisitResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var query = BuildQuery()
                .OrderByDescending(x => x.VisitDate);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<PatientVisitResponseDto>
            {
                Items = _mapper.Map<List<PatientVisitResponseDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<PagedResultDto<PatientVisitResponseDto>> GetFilteredPagedAsync(PatientVisitFilterRequestDto request)
        {
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var query = BuildQuery().AsQueryable();

            if (request.PatientId.HasValue)
                query = query.Where(x => x.PatientId == request.PatientId.Value);

            if (request.DoctorId.HasValue)
                query = query.Where(x => x.DoctorId == request.DoctorId.Value);

            if (request.ClinicId.HasValue)
                query = query.Where(x => x.ClinicId == request.ClinicId.Value);

            if (request.AppointmentId.HasValue)
                query = query.Where(x => x.AppointmentId == request.AppointmentId.Value);

            if (request.FromDate.HasValue)
            {
                var from = request.FromDate.Value.Date;
                query = query.Where(x => x.VisitDate >= from);
            }

            if (request.ToDate.HasValue)
            {
                var to = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.VisitDate <= to);
            }

            if (!string.IsNullOrWhiteSpace(request.ChiefComplaint))
            {
                var chiefComplaint = request.ChiefComplaint.Trim().ToLower();
                query = query.Where(x => x.ChiefComplaint.ToLower().Contains(chiefComplaint));
            }

            if (!string.IsNullOrWhiteSpace(request.Diagnosis))
            {
                var diagnosis = request.Diagnosis.Trim().ToLower();
                query = query.Where(x => x.Diagnosis != null && x.Diagnosis.ToLower().Contains(diagnosis));
            }

            query = query.OrderByDescending(x => x.VisitDate);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<PatientVisitResponseDto>
            {
                Items = _mapper.Map<List<PatientVisitResponseDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.PatientVisits
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new AppNotFoundException("لم يتم العثور على سجل الزيارة.");

            if (entity.AppointmentId.HasValue)
                throw new AppValidationException(new[] { "لا يمكن حذف سجل زيارة مرتبط بموعد." });

            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("تم حذف سجل الزيارة بنجاح. VisitId: {VisitId}", id);
        }

        private IQueryable<PatientVisit> BuildQuery()
        {
            return _context.PatientVisits
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.Clinic);
        }

        private async Task EnsureReferencesExistAsync(Guid patientId, Guid doctorId, Guid clinicId)
        {
            var patientExists = await _context.Patients.AnyAsync(x => x.Id == patientId);
            if (!patientExists)
                throw new AppNotFoundException("المريض غير موجود.");

            var doctorExists = await _context.Doctors.AnyAsync(x => x.Id == doctorId);
            if (!doctorExists)
                throw new AppNotFoundException("الطبيب غير موجود.");

            var clinicExists = await _context.Clinics.AnyAsync(x => x.Id == clinicId);
            if (!clinicExists)
                throw new AppNotFoundException("العيادة غير موجودة.");
        }

        private async Task<Appointment?> ValidateAppointmentAsync(
            Guid? appointmentId,
            Guid patientId,
            Guid doctorId,
            Guid clinicId,
            Guid? currentVisitId = null)
        {
            if (!appointmentId.HasValue)
                return null;

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(x => x.Id == appointmentId.Value && !x.IsDeleted);

            if (appointment == null)
                throw new AppNotFoundException("الموعد غير موجود.");

            if (appointment.PatientId != patientId)
                throw new AppValidationException(new[] { "الموعد المحدد لا يعود إلى هذا المريض." });

            if (appointment.DoctorId != doctorId)
                throw new AppValidationException(new[] { "الموعد المحدد لا يعود إلى هذا الطبيب." });

            if (appointment.ClinicId != clinicId)
                throw new AppValidationException(new[] { "الموعد المحدد لا يعود إلى هذه العيادة." });

            if (appointment.Status == AppointmentStatus.Cancelled)
                throw new AppValidationException(new[] { "لا يمكن إنشاء زيارة لموعد ملغي." });

            if (appointment.Status == AppointmentStatus.Completed)
                throw new AppValidationException(new[] { "لا يمكن إنشاء زيارة لموعد مكتمل مسبقًا." });

            var existingVisit = await _context.PatientVisits
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId.Value);

            if (existingVisit != null && existingVisit.Id != currentVisitId)
                throw new AppValidationException(new[] { "هذا الموعد مرتبط مسبقًا بسجل زيارة آخر." });

            return appointment;
        }
    }
}
