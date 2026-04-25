using AutoMapper;
using MedicalClinicSystem.Application.DTOs.Doctor;
using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Application.Mapping
{
    public class DoctorMappingProfile : Profile
    {
        public DoctorMappingProfile()
        {
            CreateMap<CreateDoctorRequestDto, Doctor>();

            CreateMap<UpdateDoctorRequestDto, Doctor>();

            CreateMap<Doctor, DoctorResponseDto>()
                .ForMember(dest => dest.ClinicName,
                    opt => opt.MapFrom(src => src.Clinic.ClinicName))
                .ForMember(dest => dest.SpecialtyName,
                    opt => opt.MapFrom(src => src.Specialty.Name));
        }
    }
}