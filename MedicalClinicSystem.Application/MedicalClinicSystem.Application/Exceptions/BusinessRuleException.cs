namespace MedicalClinicSystem.Application.Exceptions
{
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message)
            : base(message)
        {
        }
    }
}