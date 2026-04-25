using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Patient;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<CreatePatientRequestDto> _createValidator;
        private readonly IValidator<UpdatePatientRequestDto> _updateValidator;
        private readonly IMapper _mapper;

        public PatientService(
            IApplicationDbContext context,
            IValidator<CreatePatientRequestDto> createValidator,
            IValidator<UpdatePatientRequestDto> updateValidator,
            IMapper mapper)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<PatientResponseDto> CreateAsync(CreatePatientRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var patient = _mapper.Map<Patient>(dto);

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            return _mapper.Map<PatientResponseDto>(patient);
        }

        public async Task<List<PatientResponseDto>> GetAllAsync()
        {
            var patients = await _context.Patients
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return _mapper.Map<List<PatientResponseDto>>(patients);
        }

        public async Task<PatientResponseDto> GetByIdAsync(Guid id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (patient is null)
            {
                throw new NotFoundException("المريض المطلوب غير موجود.");
            }

            return _mapper.Map<PatientResponseDto>(patient);
        }

        public async Task UpdateAsync(Guid id, UpdatePatientRequestDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (patient is null)
            {
                throw new NotFoundException("المريض المطلوب غير موجود.");
            }

            _mapper.Map(dto, patient);
            patient.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task<PagedResultDto<PatientResponseDto>> GetFilteredPagedAsync(PatientFilterRequestDto request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.Patients
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var normalizedName = request.Name.Trim().ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(normalizedName));
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var normalizedPhone = request.Phone.Trim();
                query = query.Where(x => x.PhoneNumber.Contains(normalizedPhone));
            }

            query = query.OrderBy(x => x.FullName);

            var totalCount = await query.CountAsync();

            var patients = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<PatientResponseDto>>(patients);

            return new PagedResultDto<PatientResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PatientResponseDto> GetByPhoneAsync(string phoneNumber)
        {
            var patient = await _context.Patients
                .Where(x => x.PhoneNumber == phoneNumber && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (patient is null)
            {
                throw new NotFoundException("المريض غير موجود بهذا الرقم.");
            }

            return _mapper.Map<PatientResponseDto>(patient);
        }
        public async Task<PagedResultDto<PatientResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.Patients
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var patients = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<PatientResponseDto>>(patients);

            return new PagedResultDto<PatientResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        public async Task DeleteAsync(Guid id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (patient is null)
            {
                throw new NotFoundException("المريض المطلوب غير موجود.");
            }

            patient.IsDeleted = true;
            patient.DeletedAt = DateTime.UtcNow;
            patient.IsActive = false;

            await _context.SaveChangesAsync();
        }
        public async Task<List<PatientResponseDto>> SearchAsync(string? name, string? phone)
        {
            var query = _context.Patients
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(normalizedName));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var normalizedPhone = phone.Trim();
                query = query.Where(x => x.PhoneNumber.Contains(normalizedPhone));
            }


            var patients = await query
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return _mapper.Map<List<PatientResponseDto>>(patients);
        }
    }
}