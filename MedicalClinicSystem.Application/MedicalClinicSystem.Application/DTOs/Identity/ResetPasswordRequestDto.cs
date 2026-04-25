namespace MedicalClinicSystem.Application.DTOs.Identity
{
    public class ResetPasswordRequestDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
