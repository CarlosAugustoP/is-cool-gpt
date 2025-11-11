using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IsCool.DTO
{
    public record MessageRequestDTO([MaxLength(500)] string Message);
}