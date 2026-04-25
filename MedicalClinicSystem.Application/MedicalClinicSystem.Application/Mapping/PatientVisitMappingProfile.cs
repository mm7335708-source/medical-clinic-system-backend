using AutoMapper;
using MedicalClinicSystem.Application.DTOs.PatientVisit;
using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Application.Mapping
{
    public class PatientVisitMappingProfile : Profile
    {
        public PatientVisitMappingProfile()
        {
            CreateMap<CreatePatientVisitRequestDto, PatientVisit>();

            CreateMap<UpdatePatientVisitRequestDto, PatientVisit>();

            CreateMap<PatientVisit, PatientVisitResponseDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor.FullName))
                .ForMember(dest => dest.ClinicName,
                    opt => opt.MapFrom(src => src.Clinic.ClinicName));
        }
    }
}