using FluentValidation;
using MedicalClinicSystem.Application.DTOs.PatientVisit;

namespace MedicalClinicSystem.Application.Validations.PatientVisit
{
    public class CreatePatientVisitRequestDtoValidator : AbstractValidator<CreatePatientVisitRequestDto>
    {
        public CreatePatientVisitRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.PatientId)
                .NotEmpty()
                .WithMessage("يجب تحديد المريض.");

            RuleFor(x => x.DoctorId)
                .NotEmpty()
                .WithMessage("يجب تحديد الطبيب.");

            RuleFor(x => x.ClinicId)
                .NotEmpty()
                .WithMessage("يجب تحديد العيادة.");

            RuleFor(x => x.VisitDate)
                .NotEmpty()
                .WithMessage("يجب إدخال تاريخ الزيارة.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                .WithMessage("لا يمكن أن يكون تاريخ الزيارة في المستقبل.");

            RuleFor(x => x.ChiefComplaint)
                .NotEmpty()
                .WithMessage("يجب إدخال الشكوى الرئيسية.")
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("لا يمكن أن تكون الشكوى الرئيسية فارغة أو تحتوي على فراغات فقط.")
                .MaximumLength(1000)
                .WithMessage("يجب ألا تتجاوز الشكوى الرئيسية 1000 حرف.");

            RuleFor(x => x.Diagnosis)
                .MaximumLength(2000)
                .WithMessage("يجب ألا يتجاوز التشخيص 2000 حرف.")
                .Must(BeValidOptionalText)
                .WithMessage("لا يمكن أن يحتوي التشخيص على فراغات فقط.");

            RuleFor(x => x.TreatmentPlan)
                .MaximumLength(2000)
                .WithMessage("يجب ألا تتجاوز الخطة العلاجية 2000 حرف.")
                .Must(BeValidOptionalText)
                .WithMessage("لا يمكن أن تحتوي الخطة العلاجية على فراغات فقط.");

            RuleFor(x => x.Prescription)
                .MaximumLength(2000)
                .WithMessage("يجب ألا تتجاوز الوصفة الطبية 2000 حرف.")
                .Must(BeValidOptionalText)
                .WithMessage("لا يمكن أن تحتوي الوصفة الطبية على فراغات فقط.");

            RuleFor(x => x.Notes)
                .MaximumLength(3000)
                .WithMessage("يجب ألا تتجاوز الملاحظات 3000 حرف.")
                .Must(BeValidOptionalText)
                .WithMessage("لا يمكن أن تحتوي الملاحظات على فراغات فقط.");

            RuleFor(x => x.FollowUpDate)
                .GreaterThanOrEqualTo(x => x.VisitDate)
                .When(x => x.FollowUpDate.HasValue)
                .WithMessage("يجب أن يكون تاريخ المراجعة القادمة أكبر من أو يساوي تاريخ الزيارة.");
        }

        private static bool BeValidOptionalText(string? value)
        {
            return value == null || string.IsNullOrWhiteSpace(value) || value.Trim().Length > 0;
        }
    }
}