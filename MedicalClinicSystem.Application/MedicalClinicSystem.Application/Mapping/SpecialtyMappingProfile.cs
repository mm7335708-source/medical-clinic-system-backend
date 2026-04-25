using AutoMapper;
using MedicalClinicSystem.Application.DTOs.Specialty;
using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Application.Mapping
{
    public class SpecialtyMappingProfile : Profile
    {
        public SpecialtyMappingProfile()
        {
            CreateMap<CreateSpecialtyRequestDto, Specialty>();
            CreateMap<UpdateSpecialtyRequestDto, Specialty>();
            CreateMap<Specialty, SpecialtyResponseDto>();
        }
    }
}