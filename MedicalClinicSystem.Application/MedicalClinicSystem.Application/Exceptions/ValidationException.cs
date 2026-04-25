namespace MedicalClinicSystem.Application.Exceptions
{
    public class ValidationException : AppException
    {
        public IEnumerable<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }
}