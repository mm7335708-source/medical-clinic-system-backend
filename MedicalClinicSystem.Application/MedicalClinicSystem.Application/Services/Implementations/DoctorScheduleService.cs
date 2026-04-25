using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.DoctorSchedule;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<CreateDoctorScheduleRequestDto> _createValidator;
        private readonly IValidator<UpdateDoctorScheduleRequestDto> _updateValidator;
        private readonly IMapper _mapper;

        public DoctorScheduleService(
            IApplicationDbContext context,
            IValidator<CreateDoctorScheduleRequestDto> createValidator,
            IValidator<UpdateDoctorScheduleRequestDto> updateValidator,
            IMapper mapper)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<DoctorScheduleResponseDto> CreateAsync(CreateDoctorScheduleRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == dto.DoctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب المحدد غير موجود.");
            }

            var duplicatedSchedule = await _context.DoctorSchedules
      .AnyAsync(x =>
          x.DoctorId == dto.DoctorId &&
          x.DayOfWeek == (DayOfWeek)dto.DayOfWeek &&
          !x.IsDeleted);

            if (duplicatedSchedule)
            {
                throw new ConflictException("يوجد جدول لهذا الطبيب في نفس اليوم مسبقاً.");
            }

            var schedule = _mapper.Map<DoctorSchedule>(dto);

            await _context.DoctorSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            var createdSchedule = await _context.DoctorSchedules
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync(x => x.Id == schedule.Id);

            return _mapper.Map<DoctorScheduleResponseDto>(createdSchedule);
        }

        public async Task<List<DoctorScheduleResponseDto>> GetAllAsync()
        {
            var schedules = await _context.DoctorSchedules
                .Where(x => !x.IsDeleted)
                .Include(x => x.Doctor)
                .OrderBy(x => x.DayOfWeek)
                .ThenBy(x => x.StartTime)
                .ToListAsync();

            return _mapper.Map<List<DoctorScheduleResponseDto>>(schedules);
        }

        public async Task<DoctorScheduleResponseDto> GetByIdAsync(Guid id)
        {
            var schedule = await _context.DoctorSchedules
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync();

            if (schedule is null)
            {
                throw new NotFoundException("جدول الدوام المطلوب غير موجود.");
            }

            return _mapper.Map<DoctorScheduleResponseDto>(schedule);
        }

        public async Task UpdateAsync(Guid id, UpdateDoctorScheduleRequestDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var schedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (schedule is null)
            {
                throw new NotFoundException("جدول الدوام المطلوب غير موجود.");
            }

            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == dto.DoctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب المحدد غير موجود.");
            }

            var duplicateSchedule = await _context.DoctorSchedules
      .AnyAsync(x =>
          x.DoctorId == dto.DoctorId &&
          x.DayOfWeek == (DayOfWeek)dto.DayOfWeek &&
          x.Id != id &&
          !x.IsDeleted);

            if (duplicateSchedule)
            {
                throw new ConflictException("يوجد جدول آخر لهذا الطبيب في نفس اليوم.");
            }

            _mapper.Map(dto, schedule);
            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task<DoctorScheduleResponseDto> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek dayOfWeek)
        {
            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == doctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب غير موجود.");
            }

            var schedule = await _context.DoctorSchedules
                .Where(x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek && !x.IsDeleted)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync();

            if (schedule is null)
            {
                throw new NotFoundException("لا يوجد جدول دوام للطبيب في هذا اليوم.");
            }

            return _mapper.Map<DoctorScheduleResponseDto>(schedule);
        }
        public async Task DeleteAsync(Guid id)
        {
            var schedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (schedule is null)
            {
                throw new NotFoundException("جدول الدوام المطلوب غير موجود.");
            }

            schedule.IsDeleted = true;
            schedule.DeletedAt = DateTime.UtcNow;
            schedule.IsActive = false;

            await _context.SaveChangesAsync();
        }
        public async Task<List<DoctorScheduleResponseDto>> GetByDoctorAsync(Guid doctorId)
        {
            var doctorExists = await _context.Doctors
                .AnyAsync(x => x.Id == doctorId && !x.IsDeleted);

            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب غير موجود.");
            }

            var schedules = await _context.DoctorSchedules
                .Where(x => x.DoctorId == doctorId && !x.IsDeleted)
                .Include(x => x.Doctor)
                .OrderBy(x => x.DayOfWeek)
                .ThenBy(x => x.StartTime)
                .ToListAsync();

            return _mapper.Map<List<DoctorScheduleResponseDto>>(schedules);
        }
    }
}