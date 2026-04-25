using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Appointment;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using MedicalClinicSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<CreateAppointmentRequestDto> _createValidator;
        private readonly IValidator<UpdateAppointmentStatusRequestDto> _statusValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentService> _logger;
        private readonly IValidator<UpdateAppointmentRequestDto> _updateValidator;
        private readonly IValidator<CancelAppointmentRequestDto> _cancelValidator;

        public AppointmentService(
            IApplicationDbContext context,
            IValidator<CreateAppointmentRequestDto> createValidator,
            IValidator<UpdateAppointmentStatusRequestDto> statusValidator,
            IValidator<UpdateAppointmentRequestDto> updateValidator,
            IValidator<CancelAppointmentRequestDto> cancelValidator,
            IMapper mapper,
            ILogger<AppointmentService> logger)
        {
            _context = context;
            _createValidator = createValidator;
            _statusValidator = statusValidator;
            _updateValidator = updateValidator;
            _cancelValidator = cancelValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            if (!TimeSpan.TryParse(dto.StartTime, out var parsedStartTime))
            {
                throw new AppValidationException(new[] { "صيغة وقت الموعد غير صحيحة." });
            }

            var normalizedAppointmentDate = DateTime.SpecifyKind(dto.AppointmentDate.Date, DateTimeKind.Utc);

            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == dto.DoctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب المحدد غير موجود.");
            }

            var patientExists = await _context.Patients
                .AnyAsync(x => x.Id == dto.PatientId && !x.IsDeleted);

            if (!patientExists)
            {
                throw new NotFoundException("المريض المحدد غير موجود.");
            }

            var clinicExists = await _context.Clinics
                .AnyAsync(x => x.Id == dto.ClinicId && !x.IsDeleted);

            if (!clinicExists)
            {
                throw new NotFoundException("العيادة المحددة غير موجودة.");
            }

            await EnsureDoctorBelongsToClinicAsync(dto.DoctorId, dto.ClinicId);

            var appointmentDay = normalizedAppointmentDate.DayOfWeek;

            var hasValidSchedule = await _context.DoctorSchedules
                .AnyAsync(x =>
                    x.DoctorId == dto.DoctorId &&
                    x.DayOfWeek == appointmentDay &&
                    x.StartTime <= parsedStartTime &&
                    x.EndTime > parsedStartTime &&
                    !x.IsDeleted);

            if (!hasValidSchedule)
            {
                throw new BusinessRuleException("وقت الموعد خارج جدول دوام الطبيب.");
            }

            var isAlreadyBooked = await _context.Appointments
                .AnyAsync(x =>
                    x.DoctorId == dto.DoctorId &&
                    x.AppointmentDate.Date == normalizedAppointmentDate.Date &&
                    x.StartTime == parsedStartTime &&
                    !x.IsDeleted &&
                    x.Status != AppointmentStatus.Cancelled);

            if (isAlreadyBooked)
            {
                throw new ConflictException("هذا الوقت محجوز مسبقاً لهذا الطبيب.");
            }

            var appointment = _mapper.Map<Appointment>(dto);
            appointment.StartTime = parsedStartTime;
            appointment.Status = AppointmentStatus.Pending;
            appointment.AppointmentDate = normalizedAppointmentDate;

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Appointment created successfully. AppointmentId: {AppointmentId}, DoctorId: {DoctorId}, PatientId: {PatientId}, ClinicId: {ClinicId}, Date: {Date}, Time: {Time}",
                appointment.Id,
                appointment.DoctorId,
                appointment.PatientId,
                appointment.ClinicId,
                appointment.AppointmentDate,
                appointment.StartTime);

            var createdAppointment = await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync(x => x.Id == appointment.Id);

            if (createdAppointment is null)
            {
                throw new NotFoundException("فشل في جلب الموعد بعد إنشائه.");
            }

            return _mapper.Map<AppointmentResponseDto>(createdAppointment);
        }

        public async Task UpdateAsync(Guid id, UpdateAppointmentRequestDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            if (!TimeSpan.TryParse(dto.StartTime, out var parsedStartTime))
            {
                throw new AppValidationException(new[] { "صيغة وقت الموعد غير صحيحة." });
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (appointment is null)
            {
                throw new NotFoundException("الموعد المطلوب غير موجود.");
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                throw new BusinessRuleException("لا يمكن تعديل موعد مكتمل.");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new BusinessRuleException("لا يمكن تعديل موعد ملغي.");
            }

            await EnsureAppointmentIsNotLinkedToVisitAsync(appointment.Id);

            var normalizedAppointmentDate = DateTime.SpecifyKind(dto.AppointmentDate.Date, DateTimeKind.Utc);

            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == dto.DoctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب المحدد غير موجود.");
            }

            var patientExists = await _context.Patients
                .AnyAsync(x => x.Id == dto.PatientId && !x.IsDeleted);

            if (!patientExists)
            {
                throw new NotFoundException("المريض المحدد غير موجود.");
            }

            var clinicExists = await _context.Clinics
                .AnyAsync(x => x.Id == dto.ClinicId && !x.IsDeleted);

            if (!clinicExists)
            {
                throw new NotFoundException("العيادة المحددة غير موجودة.");
            }

            await EnsureDoctorBelongsToClinicAsync(dto.DoctorId, dto.ClinicId);

            var appointmentDay = normalizedAppointmentDate.DayOfWeek;

            var hasValidSchedule = await _context.DoctorSchedules
                .AnyAsync(x =>
                    x.DoctorId == dto.DoctorId &&
                    x.DayOfWeek == appointmentDay &&
                    x.StartTime <= parsedStartTime &&
                    x.EndTime > parsedStartTime &&
                    !x.IsDeleted);

            if (!hasValidSchedule)
            {
                throw new BusinessRuleException("وقت الموعد خارج جدول دوام الطبيب.");
            }

            var isAlreadyBooked = await _context.Appointments
                .AnyAsync(x =>
                    x.Id != id &&
                    x.DoctorId == dto.DoctorId &&
                    x.AppointmentDate.Date == normalizedAppointmentDate.Date &&
                    x.StartTime == parsedStartTime &&
                    !x.IsDeleted &&
                    x.Status != AppointmentStatus.Cancelled);

            if (isAlreadyBooked)
            {
                throw new ConflictException("هذا الوقت محجوز مسبقاً لهذا الطبيب.");
            }

            appointment.DoctorId = dto.DoctorId;
            appointment.PatientId = dto.PatientId;
            appointment.ClinicId = dto.ClinicId;
            appointment.AppointmentDate = normalizedAppointmentDate;
            appointment.StartTime = parsedStartTime;
            appointment.Notes = dto.Notes;
            appointment.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Appointment updated successfully. AppointmentId: {AppointmentId}, DoctorId: {DoctorId}, PatientId: {PatientId}, ClinicId: {ClinicId}, Date: {Date}, Time: {Time}",
                appointment.Id,
                appointment.DoctorId,
                appointment.PatientId,
                appointment.ClinicId,
                appointment.AppointmentDate,
                appointment.StartTime);

            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(Guid id, CancelAppointmentRequestDto dto)
        {
            var validationResult = await _cancelValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (appointment is null)
            {
                throw new NotFoundException("الموعد المطلوب غير موجود.");
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                throw new BusinessRuleException("لا يمكن إلغاء موعد مكتمل.");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new BusinessRuleException("الموعد ملغي بالفعل.");
            }

            await EnsureAppointmentIsNotLinkedToVisitAsync(appointment.Id);

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = dto.Reason;
            appointment.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Appointment cancelled successfully. AppointmentId: {AppointmentId}, Reason: {Reason}",
                appointment.Id,
                appointment.CancellationReason);

            await _context.SaveChangesAsync();
        }

        public async Task<List<AppointmentResponseDto>> GetAllAsync()
        {
            var appointments = await _context.Appointments
                .Where(x => !x.IsDeleted)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .OrderBy(x => x.AppointmentDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();

            return _mapper.Map<List<AppointmentResponseDto>>(appointments);
        }

        public async Task<AppointmentResponseDto> GetByIdAsync(Guid id)
        {
            var appointment = await _context.Appointments
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync();

            if (appointment is null)
            {
                throw new NotFoundException("الموعد المطلوب غير موجود.");
            }

            return _mapper.Map<AppointmentResponseDto>(appointment);
        }

        public async Task UpdateStatusAsync(Guid id, UpdateAppointmentStatusRequestDto dto)
        {
            var validationResult = await _statusValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (appointment is null)
            {
                throw new NotFoundException("الموعد غير موجود.");
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                throw new BusinessRuleException("لا يمكن تعديل موعد مكتمل.");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new BusinessRuleException("الموعد الملغي لا يمكن تعديله.");
            }

            if (dto.Status == AppointmentStatus.Completed)
            {
                throw new BusinessRuleException("لا يمكن تحويل الموعد إلى مكتمل يدويًا. يتم ذلك فقط عند إنشاء سجل زيارة.");
            }

            if (dto.Status == AppointmentStatus.Cancelled)
            {
                throw new BusinessRuleException("استخدم عملية الإلغاء (cancel) لتحديد سبب الإلغاء.");
            }

            appointment.Status = dto.Status;
            appointment.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Appointment status updated. AppointmentId: {AppointmentId}, NewStatus: {Status}",
                appointment.Id,
                appointment.Status);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (appointment is null)
            {
                throw new NotFoundException("الموعد المطلوب غير موجود.");
            }

            await EnsureAppointmentIsNotLinkedToVisitAsync(appointment.Id);

            appointment.IsDeleted = true;
            appointment.DeletedAt = DateTime.UtcNow;
            appointment.IsActive = false;

            _logger.LogInformation(
                "Appointment soft deleted. AppointmentId: {AppointmentId}",
                appointment.Id);

            await _context.SaveChangesAsync();
        }

        public async Task<List<AppointmentResponseDto>> GetByDateAsync(DateTime date)
        {
            var normalizedDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            var appointments = await _context.Appointments
                .Where(x => !x.IsDeleted && x.AppointmentDate.Date == normalizedDate.Date)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .OrderBy(x => x.StartTime)
                .ToListAsync();

            return _mapper.Map<List<AppointmentResponseDto>>(appointments);
        }

        public async Task<List<AppointmentResponseDto>> GetByDoctorAsync(Guid doctorId)
        {
            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == doctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب غير موجود.");
            }

            var appointments = await _context.Appointments
                .Where(x => x.DoctorId == doctorId && !x.IsDeleted)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .OrderBy(x => x.AppointmentDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();

            return _mapper.Map<List<AppointmentResponseDto>>(appointments);
        }

        public async Task<List<AppointmentResponseDto>> GetByStatusAsync(AppointmentStatus status)
        {
            var appointments = await _context.Appointments
                .Where(x => x.Status == status && !x.IsDeleted)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .OrderBy(x => x.AppointmentDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();

            return _mapper.Map<List<AppointmentResponseDto>>(appointments);
        }

        public async Task<List<AvailableSlotResponseDto>> GetAvailableSlotsAsync(Guid doctorId, DateTime date)
        {
            var normalizedDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == doctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب غير موجود.");
            }

            var dayOfWeek = normalizedDate.DayOfWeek;

            var schedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(x =>
                    x.DoctorId == doctorId &&
                    x.DayOfWeek == dayOfWeek &&
                    !x.IsDeleted);

            if (schedule is null)
            {
                throw new NotFoundException("لا يوجد جدول دوام للطبيب في هذا اليوم.");
            }

            var bookedSlots = await _context.Appointments
                .Where(x =>
                    x.DoctorId == doctorId &&
                    x.AppointmentDate.Date == normalizedDate.Date &&
                    !x.IsDeleted &&
                    x.Status != AppointmentStatus.Cancelled)
                .Select(x => x.StartTime)
                .ToListAsync();

            var availableSlots = new List<AvailableSlotResponseDto>();
            var current = schedule.StartTime;
            var slotDuration = TimeSpan.FromMinutes(30);

            while (current < schedule.EndTime)
            {
                if (!bookedSlots.Contains(current))
                {
                    availableSlots.Add(new AvailableSlotResponseDto
                    {
                        Time = current.ToString()
                    });
                }

                current = current.Add(slotDuration);
            }

            return availableSlots;
        }

        public async Task<PagedResultDto<AppointmentResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.Appointments
                .Where(x => !x.IsDeleted)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .OrderByDescending(x => x.AppointmentDate)
                .ThenBy(x => x.StartTime)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var appointments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<AppointmentResponseDto>>(appointments);

            return new PagedResultDto<AppointmentResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PagedResultDto<AppointmentResponseDto>> GetFilteredPagedAsync(AppointmentFilterRequestDto request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.Appointments
                .Where(x => !x.IsDeleted)
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Clinic)
                .AsQueryable();

            if (request.DoctorId.HasValue)
            {
                query = query.Where(x => x.DoctorId == request.DoctorId.Value);
            }

            if (request.ClinicId.HasValue)
            {
                query = query.Where(x => x.ClinicId == request.ClinicId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.Date.HasValue)
            {
                var normalizedDate = DateTime.SpecifyKind(request.Date.Value.Date, DateTimeKind.Utc);
                query = query.Where(x => x.AppointmentDate.Date == normalizedDate.Date);
            }

            query = query
                .OrderByDescending(x => x.AppointmentDate)
                .ThenBy(x => x.StartTime);

            var totalCount = await query.CountAsync();

            var appointments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<AppointmentResponseDto>>(appointments);

            return new PagedResultDto<AppointmentResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        private async Task EnsureAppointmentIsNotLinkedToVisitAsync(Guid appointmentId)
        {
            var hasVisit = await _context.PatientVisits
                .AnyAsync(x => x.AppointmentId == appointmentId);

            if (hasVisit)
            {
                throw new BusinessRuleException("لا يمكن تنفيذ هذه العملية لأن الموعد مرتبط بسجل زيارة.");
            }
        }

        private async Task EnsureDoctorBelongsToClinicAsync(Guid doctorId, Guid clinicId)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(x => x.Id == doctorId && !x.IsDeleted);

            if (doctor == null)
            {
                throw new NotFoundException("الطبيب المحدد غير موجود.");
            }

            if (doctor.ClinicId != clinicId)
            {
                throw new BusinessRuleException("الطبيب المحدد لا ينتمي إلى العيادة المحددة.");
            }
        }
    }
}
