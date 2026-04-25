using AutoMapper;
using MedicalClinicSystem.Application.DTOs.Patient;
using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Application.Mapping
{
    public class PatientMappingProfile : Profile
    {
        public PatientMappingProfile()
        {
            CreateMap<CreatePatientRequestDto, Patient>();
            CreateMap<UpdatePatientRequestDto, Patient>();
            CreateMap<Patient, PatientResponseDto>();
        }
    }
}