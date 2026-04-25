using AutoMapper;
using MedicalClinicSystem.Application.DTOs.Identity;
using MedicalClinicSystem.Domain.Entities.Identity;

namespace MedicalClinicSystem.Application.Mapping
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Role, RoleResponseDto>();
        }
    }
}
