using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Validations.Identity
{
    public class UpdateUserStatusRequestDtoValidator : AbstractValidator<UpdateUserStatusRequestDto>
    {
        public UpdateUserStatusRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("يجب تحديد حالة المستخدم (IsActive).");
        }
    }
}
