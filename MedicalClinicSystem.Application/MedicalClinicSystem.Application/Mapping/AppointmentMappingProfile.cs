using AutoMapper;
using MedicalClinicSystem.Application.DTOs.Appointment;
using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Application.Mapping
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<CreateAppointmentRequestDto, Appointment>()
                .ForMember(dest => dest.StartTime, opt => opt.Ignore());

            CreateMap<UpdateAppointmentRequestDto, Appointment>()
                .ForMember(dest => dest.StartTime, opt => opt.Ignore());

            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : "N/A"))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : "N/A"))
                .ForMember(dest => dest.ClinicName,
                    opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.ClinicName : "N/A"))
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src => src.StartTime.ToString()))
                .ForMember(dest => dest.CancellationReason,
                    opt => opt.MapFrom(src => src.CancellationReason));
        }
    }
}