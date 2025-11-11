using IsCool.Models;

namespace IsCool.DTO
{
    public record ForgotPasswordDTO
    (
        string Email, string Otp, string NewPassword
    );
}