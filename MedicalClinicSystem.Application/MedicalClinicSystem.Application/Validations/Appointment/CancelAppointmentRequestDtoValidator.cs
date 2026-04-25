using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Appointment;

namespace MedicalClinicSystem.Application.Validations.Appointment
{
    public class CancelAppointmentRequestDtoValidator : AbstractValidator<CancelAppointmentRequestDto>
    {
        public CancelAppointmentRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("سبب الإلغاء مطلوب.")
                .MaximumLength(1000).WithMessage("سبب الإلغاء يجب أن لا يتجاوز 1000 حرف.");
        }
    }
}