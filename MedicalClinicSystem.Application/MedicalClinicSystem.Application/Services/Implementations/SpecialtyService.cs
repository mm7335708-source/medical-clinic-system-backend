using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Specialty;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<CreateSpecialtyRequestDto> _createValidator;
        private readonly IValidator<UpdateSpecialtyRequestDto> _updateValidator;
        private readonly IMapper _mapper;

        public SpecialtyService(
            IApplicationDbContext context,
            IValidator<CreateSpecialtyRequestDto> createValidator,
            IValidator<UpdateSpecialtyRequestDto> updateValidator,
            IMapper mapper)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<SpecialtyResponseDto> CreateAsync(CreateSpecialtyRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var existingSpecialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Name == dto.Name && !x.IsDeleted);

            if (existingSpecialty is not null)
            {
                throw new ConflictException("هذا الاختصاص موجود مسبقاً.");
            }

            var specialty = _mapper.Map<Specialty>(dto);

            await _context.Specialties.AddAsync(specialty);
            await _context.SaveChangesAsync();

            return _mapper.Map<SpecialtyResponseDto>(specialty);
        }

        public async Task<List<SpecialtyResponseDto>> GetAllAsync()
        {
            var specialties = await _context.Specialties
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return _mapper.Map<List<SpecialtyResponseDto>>(specialties);
        }

        public async Task<SpecialtyResponseDto> GetByIdAsync(Guid id)
        {
            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (specialty is null)
            {
                throw new NotFoundException("الاختصاص المطلوب غير موجود.");
            }

            return _mapper.Map<SpecialtyResponseDto>(specialty);
        }

        public async Task UpdateAsync(Guid id, UpdateSpecialtyRequestDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (specialty is null)
            {
                throw new NotFoundException("الاختصاص المطلوب غير موجود.");
            }

            var duplicateSpecialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Name == dto.Name && x.Id != id && !x.IsDeleted);

            if (duplicateSpecialty is not null)
            {
                throw new ConflictException("يوجد اختصاص آخر بنفس الاسم.");
            }

            _mapper.Map(dto, specialty);
            specialty.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (specialty is null)
            {
                throw new NotFoundException("الاختصاص المطلوب غير موجود.");
            }

            specialty.IsDeleted = true;
            specialty.DeletedAt = DateTime.UtcNow;
            specialty.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}