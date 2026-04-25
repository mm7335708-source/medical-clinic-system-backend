namespace MedicalClinicSystem.Application.DTOs.Common
{
    public class ApiErrorResponseDto
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string? ExceptionType { get; set; }
        public object? Details { get; set; }
    }
}