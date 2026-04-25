using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Validations.Identity
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserNameOrEmail)
                .NotEmpty()
                .WithMessage("يجب إدخال اسم المستخدم أو البريد الإلكتروني.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("يجب إدخال كلمة المرور.");
        }
    }
}