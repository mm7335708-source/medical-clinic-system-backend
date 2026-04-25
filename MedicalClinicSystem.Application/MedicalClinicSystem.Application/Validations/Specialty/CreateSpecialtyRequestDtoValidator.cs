using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Specialty;

namespace MedicalClinicSystem.Application.Validations.Specialty
{
    public class CreateSpecialtyRequestDtoValidator : AbstractValidator<CreateSpecialtyRequestDto>
    {
        public CreateSpecialtyRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم الاختصاص مطلوب.")
                .MaximumLength(100).WithMessage("اسم الاختصاص يجب أن لا يتجاوز 100 حرف.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("الوصف يجب أن لا يتجاوز 500 حرف.");
        }
    }
}