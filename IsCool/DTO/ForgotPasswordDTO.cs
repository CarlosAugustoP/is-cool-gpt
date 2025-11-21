using System.ComponentModel.DataAnnotations;
using IsCool.Models;

namespace IsCool.DTO
{
    public record ForgotPasswordDTO
    (
        [EmailAddress]
        string Email,

        [MaxLength(6)]
        string Otp,

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain both letters and numbers.")]
        string NewPassword
    );
}