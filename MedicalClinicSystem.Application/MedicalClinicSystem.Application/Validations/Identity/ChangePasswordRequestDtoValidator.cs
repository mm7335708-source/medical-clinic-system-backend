using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Validations.Identity
{
    public class ChangePasswordRequestDtoValidator : AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("يجب إدخال كلمة المرور الحالية.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("يجب إدخال كلمة المرور الجديدة.")
                .MinimumLength(6)
                .WithMessage("يجب ألا تقل كلمة المرور الجديدة عن 6 أحرف.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("يجب تأكيد كلمة المرور الجديدة.")
                .Equal(x => x.NewPassword)
                .WithMessage("تأكيد كلمة المرور غير مطابق.");
        }
    }
}