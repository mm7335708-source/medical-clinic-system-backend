using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Clinic;

namespace MedicalClinicSystem.Application.Validations.Clinic
{
    public class UpdateClinicRequestDtoValidator : AbstractValidator<UpdateClinicRequestDto>
    {
        public UpdateClinicRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.ClinicName)
                .NotEmpty().WithMessage("اسم العيادة مطلوب")
                .MaximumLength(150).WithMessage("اسم العيادة يجب أن لا يتجاوز 150 حرف");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("العنوان مطلوب")
                .MaximumLength(250).WithMessage("العنوان يجب أن لا يتجاوز 250 حرف");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("المدينة مطلوبة")
                .MaximumLength(100).WithMessage("اسم المدينة يجب أن لا يتجاوز 100 حرف");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب")
                .MaximumLength(20).WithMessage("رقم الهاتف يجب أن لا يتجاوز 20 حرف");
        }
    }
}