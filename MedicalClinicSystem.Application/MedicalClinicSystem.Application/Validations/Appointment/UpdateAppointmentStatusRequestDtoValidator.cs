using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Appointment;

namespace MedicalClinicSystem.Application.Validations.Appointment
{
    public class UpdateAppointmentStatusRequestDtoValidator : AbstractValidator<UpdateAppointmentStatusRequestDto>
    {
        public UpdateAppointmentStatusRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("حالة الموعد غير صحيحة.");
        }
    }
}