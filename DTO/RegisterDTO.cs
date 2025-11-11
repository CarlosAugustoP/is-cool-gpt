using IsCool.Models;

namespace IsCool.DTO
{
    public record RegisterDTO
    (
        string Email,
        string Name,
        string Password,
        string UserName,
        string StudentOf,
        Language Language
    );
}