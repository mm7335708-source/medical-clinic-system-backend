using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Doctor;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<CreateDoctorRequestDto> _createValidator;
        private readonly IValidator<UpdateDoctorRequestDto> _updateValidator;
        private readonly IMapper _mapper;

        public DoctorService(
            IApplicationDbContext context,
            IValidator<CreateDoctorRequestDto> createValidator,
            IValidator<UpdateDoctorRequestDto> updateValidator,
            IMapper mapper)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<DoctorResponseDto>> GetFilteredPagedAsync(DoctorFilterRequestDto request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.Doctors
                .Where(x => !x.IsDeleted)
                .Include(x => x.Clinic)
                .Include(x => x.Specialty)
                .AsQueryable();

            if (request.ClinicId.HasValue)
            {
                query = query.Where(x => x.ClinicId == request.ClinicId.Value);
            }

            if (request.SpecialtyId.HasValue)
            {
                query = query.Where(x => x.SpecialtyId == request.SpecialtyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var normalizedName = request.Name.Trim().ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(normalizedName));
            }

            query = query.OrderBy(x => x.FullName);

            var totalCount = await query.CountAsync();

            var doctors = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<DoctorResponseDto>>(doctors);

            return new PagedResultDto<DoctorResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        public async Task<PagedResultDto<DoctorResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.Doctors
                .Where(x => !x.IsDeleted)
                .Include(x => x.Clinic)
                .Include(x => x.Specialty)
                .OrderBy(x => x.FullName)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var doctors = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<DoctorResponseDto>>(doctors);

            return new PagedResultDto<DoctorResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        public async Task<List<DoctorResponseDto>> GetBySpecialtyAsync(Guid specialtyId)
        {
            var specialtyExists = await _context.Specialties
                .AnyAsync(x => x.Id == specialtyId && !x.IsDeleted);

            if (!specialtyExists)
            {
                throw new NotFoundException("الاختصاص غير موجود.");
            }

            var doctors = await _context.Doctors
                .Where(x => x.SpecialtyId == specialtyId && !x.IsDeleted)
                .Include(x => x.Clinic)
                .Include(x => x.Specialty)
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return _mapper.Map<List<DoctorResponseDto>>(doctors);
        }
        public async Task<List<DoctorResponseDto>> GetByClinicAsync(Guid clinicId)
        {
            var clinicExists = await _context.Clinics
                .AnyAsync(x => x.Id == clinicId && !x.IsDeleted);

            if (!clinicExists)
            {
                throw new NotFoundException("العيادة غير موجودة.");
            }

            var doctors = await _context.Doctors
                .Where(x => x.ClinicId == clinicId && !x.IsDeleted)
                .Include(x => x.Clinic)
                .Include(x => x.Specialty)
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return _mapper.Map<List<DoctorResponseDto>>(doctors);
        }
        public async Task<DoctorResponseDto> CreateAsync(CreateDoctorRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var clinicExists = await _context.Clinics
                .AnyAsync(x => x.Id == dto.ClinicId && !x.IsDeleted);

            if (!clinicExists)
            {
                throw new NotFoundException("العيادة المحددة غير موجودة.");
            }

            var specialtyExists = await _context.Specialties
                .AnyAsync(x => x.Id == dto.SpecialtyId && !x.IsDeleted);

            if (!specialtyExists)
            {
                throw new NotFoundException("الاختصاص المحدد غير موجود.");
            }

            var doctor = _mapper.Map<Doctor>(dto);

            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            var createdDoctor = await _context.Doctors
                .Include(x => x.Clinic)
                .Include(x => x.Specialty)
                .FirstOrDefaultAsync(x => x.Id == doctor.Id);

            return _mapper.Map<DoctorResponseDto>(createdDoctor);
        }

        public async Task<List<DoctorResponseDto>> GetAllAsync()
        {
            var doctors = await _context.Doctors
                .Where(x => !x.IsDeleted)
                .Include(x => x.Clinic)
                .Include(x => x.Specialty)
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return _mapper.Map<List<DoctorResponseDto>>(doctors);
        }

        public async Task<DoctorResponseDto> GetByIdAsync(Guid id)
        {
            var doctor = await _context.Doctors
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.Clinic)
                .Include(x => x.Specialty)
                .FirstOrDefaultAsync();

            if (doctor is null)
            {
                throw new NotFoundException("الطبيب المطلوب غير موجود.");
            }

            return _mapper.Map<DoctorResponseDto>(doctor);
        }

        public async Task UpdateAsync(Guid id, UpdateDoctorRequestDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (doctor is null)
            {
                throw new NotFoundException("الطبيب المطلوب غير موجود.");
            }

            var clinicExists = await _context.Clinics
                .AnyAsync(x => x.Id == dto.ClinicId && !x.IsDeleted);

            if (!clinicExists)
            {
                throw new NotFoundException("العيادة المحددة غير موجودة.");
            }

            var specialtyExists = await _context.Specialties
                .AnyAsync(x => x.Id == dto.SpecialtyId && !x.IsDeleted);

            if (!specialtyExists)
            {
                throw new NotFoundException("الاختصاص المحدد غير موجود.");
            }

            _mapper.Map(dto, doctor);
            doctor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (doctor is null)
            {
                throw new NotFoundException("الطبيب المطلوب غير موجود.");
            }

            doctor.IsDeleted = true;
            doctor.DeletedAt = DateTime.UtcNow;
            doctor.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}