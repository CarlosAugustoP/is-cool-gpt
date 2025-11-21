using System.ComponentModel.DataAnnotations;
using IsCool.Models;

namespace IsCool.DTO
{
    public record RegisterDTO
    (
        [EmailAddress]
        string Email,
        [MaxLength(20)]
        string Name,
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain both letters and numbers.")]
        string Password,
        [MaxLength(10)]
        string UserName,
        [MaxLength(10)]
        string StudentOf,
        Language Language
    );
}