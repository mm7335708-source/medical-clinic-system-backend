using FluentValidation;
using MedicalClinicSystem.Application.DTOs.DoctorSchedule;

namespace MedicalClinicSystem.Application.Validations.DoctorSchedule
{
    public class CreateDoctorScheduleRequestDtoValidator : AbstractValidator<CreateDoctorScheduleRequestDto>
    {
        public CreateDoctorScheduleRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("معرف الطبيب مطلوب.");

            RuleFor(x => x.DayOfWeek)
                .InclusiveBetween(0, 6).WithMessage("اليوم يجب أن يكون بين 0 و 6.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("وقت البداية مطلوب.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("وقت النهاية مطلوب.");

            RuleFor(x => x)
                .Must(x => x.EndTime > x.StartTime)
                .WithMessage("وقت النهاية يجب أن يكون أكبر من وقت البداية.");
        }
    }
}