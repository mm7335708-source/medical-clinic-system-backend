using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Clinic;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class ClinicService : IClinicService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<CreateClinicRequestDto> _createValidator;
        private readonly IValidator<UpdateClinicRequestDto> _updateValidator;
        private readonly IMapper _mapper;

        public ClinicService(
            IApplicationDbContext context,
            IValidator<CreateClinicRequestDto> createValidator,
            IValidator<UpdateClinicRequestDto> updateValidator,
            IMapper mapper)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<ClinicResponseDto> CreateAsync(CreateClinicRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var clinic = _mapper.Map<Clinic>(dto);

            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClinicResponseDto>(clinic);
        }

        public async Task<List<ClinicResponseDto>> GetAllAsync()
        {
            var clinics = await _context.Clinics
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ClinicName)
                .ToListAsync();

            return _mapper.Map<List<ClinicResponseDto>>(clinics);
        }

        public async Task<ClinicResponseDto> GetByIdAsync(Guid id)
        {
            var clinic = await _context.Clinics
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (clinic is null)
            {
                throw new NotFoundException("العيادة المطلوبة غير موجودة.");
            }

            return _mapper.Map<ClinicResponseDto>(clinic);
        }

        public async Task UpdateAsync(Guid id, UpdateClinicRequestDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var clinic = await _context.Clinics
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (clinic is null)
            {
                throw new NotFoundException("العيادة المطلوبة غير موجودة.");
            }

            _mapper.Map(dto, clinic);

            clinic.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var clinic = await _context.Clinics
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (clinic is null)
            {
                throw new NotFoundException("العيادة المطلوبة غير موجودة.");
            }

            clinic.IsDeleted = true;
            clinic.DeletedAt = DateTime.UtcNow;
            clinic.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}