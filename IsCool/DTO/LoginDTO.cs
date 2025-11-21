using System.ComponentModel.DataAnnotations;

namespace IsCool.DTO
{
    public class LoginRequest
    {
        [EmailAddress]
        public required string Email { get; set; } 
        
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", 
            ErrorMessage = "Password must be at least 8 characters long and contain both letters and numbers.")]
        public required string Password { get; set; }
    }
}