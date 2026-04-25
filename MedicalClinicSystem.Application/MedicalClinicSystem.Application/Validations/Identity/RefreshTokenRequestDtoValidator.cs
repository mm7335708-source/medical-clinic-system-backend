using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Identity;

namespace MedicalClinicSystem.Application.Validations.Identity
{
    public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
