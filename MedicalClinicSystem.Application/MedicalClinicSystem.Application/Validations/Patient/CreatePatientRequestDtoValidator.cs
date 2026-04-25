using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Patient;

namespace MedicalClinicSystem.Application.Validations.Patient
{
    public class CreatePatientRequestDtoValidator : AbstractValidator<CreatePatientRequestDto>
    {
        public CreatePatientRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("اسم المريض مطلوب.")
                .MaximumLength(150).WithMessage("اسم المريض يجب أن لا يتجاوز 150 حرف.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب.")
                .MaximumLength(20).WithMessage("رقم الهاتف يجب أن لا يتجاوز 20 حرف.");

            RuleFor(x => x.Gender)
                .InclusiveBetween(1, 2).WithMessage("الجنس يجب أن يكون 1 أو 2.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("تاريخ الولادة يجب أن يكون أقل من التاريخ الحالي.");

            RuleFor(x => x.Address)
                .MaximumLength(250).WithMessage("العنوان يجب أن لا يتجاوز 250 حرف.");
        }
    }
}