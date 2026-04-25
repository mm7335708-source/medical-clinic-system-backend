namespace MedicalClinicSystem.Application.DTOs.Identity
{
    public class LoginRequestDto
    {
        public string UserNameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}