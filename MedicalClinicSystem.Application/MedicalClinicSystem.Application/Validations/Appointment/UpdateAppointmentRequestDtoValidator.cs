using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Appointment;

namespace MedicalClinicSystem.Application.Validations.Appointment
{
    public class UpdateAppointmentRequestDtoValidator : AbstractValidator<UpdateAppointmentRequestDto>
    {
        public UpdateAppointmentRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("معرف الطبيب مطلوب.");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("معرف المريض مطلوب.");

            RuleFor(x => x.ClinicId)
                .NotEmpty().WithMessage("معرف العيادة مطلوب.");

            RuleFor(x => x.AppointmentDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("تاريخ الموعد يجب أن يكون اليوم أو بعده.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("وقت الموعد مطلوب.")
                .Must(BeValidTimeSpan).WithMessage("صيغة وقت الموعد غير صحيحة. استخدم مثلاً 10:00:00");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("الملاحظات يجب أن لا تتجاوز 1000 حرف.");
        }

        private bool BeValidTimeSpan(string startTime)
        {
            return TimeSpan.TryParse(startTime, out _);
        }
    }
}