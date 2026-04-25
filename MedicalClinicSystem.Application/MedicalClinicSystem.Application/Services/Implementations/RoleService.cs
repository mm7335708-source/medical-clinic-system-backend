using AutoMapper;
using MedicalClinicSystem.Application.DTOs.Identity;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoleService(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RoleResponseDto>> GetAllAsync()
        {
            var roles = await _context.Roles
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return _mapper.Map<List<RoleResponseDto>>(roles);
        }

        public async Task<RoleResponseDto> GetByIdAsync(Guid id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted && x.IsActive);

            if (role == null)
            {
                throw new NotFoundException("Role was not found.");
            }

            return _mapper.Map<RoleResponseDto>(role);
        }
    }
}
