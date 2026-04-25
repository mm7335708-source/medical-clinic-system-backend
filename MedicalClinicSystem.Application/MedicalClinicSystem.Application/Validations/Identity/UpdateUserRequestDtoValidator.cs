using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Validations.Identity
{
    public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("يجب إدخال الاسم الكامل.")
                .MaximumLength(200)
                .WithMessage("يجب ألا يتجاوز الاسم الكامل 200 حرف.");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("يجب إدخال اسم المستخدم.")
                .MaximumLength(100)
                .WithMessage("يجب ألا يتجاوز اسم المستخدم 100 حرف.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage("يجب ألا يتجاوز رقم الهاتف 20 حرف.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("يجب تحديد الدور.");
        }
    }
}