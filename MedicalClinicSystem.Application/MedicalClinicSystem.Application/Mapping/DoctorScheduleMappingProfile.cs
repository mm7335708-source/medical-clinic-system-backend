using AutoMapper;
using MedicalClinicSystem.Application.DTOs.DoctorSchedule;
using MedicalClinicSystem.Domain.Entities;

namespace MedicalClinicSystem.Application.Mapping
{
    public class DoctorScheduleMappingProfile : Profile
    {
        public DoctorScheduleMappingProfile()
        {
            CreateMap<CreateDoctorScheduleRequestDto, DoctorSchedule>()
                .ForMember(dest => dest.DayOfWeek,
                    opt => opt.MapFrom(src => (DayOfWeek)src.DayOfWeek));

            CreateMap<UpdateDoctorScheduleRequestDto, DoctorSchedule>()
                .ForMember(dest => dest.DayOfWeek,
                    opt => opt.MapFrom(src => (DayOfWeek)src.DayOfWeek));

            CreateMap<DoctorSchedule, DoctorScheduleResponseDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor.FullName))
                .ForMember(dest => dest.DayOfWeek,
                    opt => opt.MapFrom(src => (int)src.DayOfWeek));
        }
    } h
}