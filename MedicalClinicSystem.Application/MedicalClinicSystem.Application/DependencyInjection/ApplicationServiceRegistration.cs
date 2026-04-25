using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Appointment;
using MedicalClinicSystem.Application.DTOs.Clinic;
using MedicalClinicSystem.Application.DTOs.Doctor;
using MedicalClinicSystem.Application.DTOs.DoctorSchedule;
using MedicalClinicSystem.Application.DTOs.Identity;
using MedicalClinicSystem.Application.DTOs.Patient;
using MedicalClinicSystem.Application.DTOs.PatientVisit;
using MedicalClinicSystem.Application.DTOs.Specialty;
using MedicalClinicSystem.Application.Mapping;
using MedicalClinicSystem.Application.Services.Implementations;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Application.Validations.Appointment;
using MedicalClinicSystem.Application.Validations.Clinic;
using MedicalClinicSystem.Application.Validations.Doctor;
using MedicalClinicSystem.Application.Validations.DoctorSchedule;
using MedicalClinicSystem.Application.Validations.Identity;
using MedicalClinicSystem.Application.Validations.Patient;
using MedicalClinicSystem.Application.Validations.PatientVisit;
using MedicalClinicSystem.Application.Validations.Specialty;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalClinicSystem.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(ClinicMappingProfile).Assembly);

            // Clinic
            services.AddScoped<IValidator<CreateClinicRequestDto>, CreateClinicRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateClinicRequestDto>, UpdateClinicRequestDtoValidator>();
            services.AddScoped<IClinicService, ClinicService>();

            // Specialty
            services.AddScoped<IValidator<CreateSpecialtyRequestDto>, CreateSpecialtyRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateSpecialtyRequestDto>, UpdateSpecialtyRequestDtoValidator>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();

            // Doctor
            services.AddScoped<IValidator<CreateDoctorRequestDto>, CreateDoctorRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateDoctorRequestDto>, UpdateDoctorRequestDtoValidator>();
            services.AddScoped<IDoctorService, DoctorService>();

            // Patient
            services.AddScoped<IValidator<CreatePatientRequestDto>, CreatePatientRequestDtoValidator>();
            services.AddScoped<IValidator<UpdatePatientRequestDto>, UpdatePatientRequestDtoValidator>();
            services.AddScoped<IPatientService, PatientService>();

            // DoctorSchedule
            services.AddScoped<IValidator<CreateDoctorScheduleRequestDto>, CreateDoctorScheduleRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateDoctorScheduleRequestDto>, UpdateDoctorScheduleRequestDtoValidator>();
            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();

            // Appointment
            services.AddScoped<IValidator<CreateAppointmentRequestDto>, CreateAppointmentRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateAppointmentStatusRequestDto>, UpdateAppointmentStatusRequestDtoValidator>();
            services.AddScoped<IValidator<CancelAppointmentRequestDto>, CancelAppointmentRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateAppointmentRequestDto>, UpdateAppointmentRequestDtoValidator>();
            services.AddScoped<IAppointmentService, AppointmentService>();

            // PatientVisit
            services.AddScoped<IValidator<CreatePatientVisitRequestDto>, CreatePatientVisitRequestDtoValidator>();
            services.AddScoped<IValidator<UpdatePatientVisitRequestDto>, UpdatePatientVisitRequestDtoValidator>();
            services.AddScoped<IPatientVisitService, PatientVisitService>();

            // Dashboard
            services.AddScoped<IDashboardService, DashboardService>();

            // User
            services.AddScoped<IValidator<CreateUserRequestDto>, CreateUserRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateUserRequestDto>, UpdateUserRequestDtoValidator>();
            services.AddScoped<IValidator<UpdateUserStatusRequestDto>, UpdateUserStatusRequestDtoValidator>();
            services.AddScoped<IValidator<ChangePasswordRequestDto>, ChangePasswordRequestDtoValidator>();
            services.AddScoped<IValidator<ResetPasswordRequestDto>, ResetPasswordRequestDtoValidator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            // Auth
            services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
            services.AddScoped<IValidator<RefreshTokenRequestDto>, RefreshTokenRequestDtoValidator>();
            services.AddScoped<IValidator<LogoutRequestDto>, LogoutRequestDtoValidator>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
