using AutoMapper;
using MedicalClinicSystem.Application.DTOs.Clinic;
using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Application.Mapping
{
    public class ClinicMappingProfile : Profile
    {
        public ClinicMappingProfile()
        {
            CreateMap<CreateClinicRequestDto, Clinic>();

            CreateMap<UpdateClinicRequestDto, Clinic>();

            CreateMap<Clinic, ClinicResponseDto>();
        }
    }
}