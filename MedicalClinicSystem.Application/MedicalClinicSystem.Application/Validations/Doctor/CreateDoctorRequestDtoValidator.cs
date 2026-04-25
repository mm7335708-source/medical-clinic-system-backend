using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Doctor;

namespace MedicalClinicSystem.Application.Validations.Doctor
{
    public class CreateDoctorRequestDtoValidator : AbstractValidator<CreateDoctorRequestDto>
    {
        public CreateDoctorRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("اسم الدكتور مطلوب.")
                .MaximumLength(150).WithMessage("اسم الدكتور يجب أن لا يتجاوز 150 حرف.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب.")
                .MaximumLength(20).WithMessage("رقم الهاتف يجب أن لا يتجاوز 20 حرف.");

            RuleFor(x => x.ExperienceYears)
                .GreaterThanOrEqualTo(0).WithMessage("سنوات الخبرة يجب أن تكون صفر أو أكثر.");

            RuleFor(x => x.ClinicId)
                .NotEmpty().WithMessage("معرف العيادة مطلوب.");

            RuleFor(x => x.SpecialtyId)
                .NotEmpty().WithMessage("معرف الاختصاص مطلوب.");
        }
    }
}