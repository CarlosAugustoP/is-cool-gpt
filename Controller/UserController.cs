using IsCool.Abstractions;
using IsCool.DTO;
using IsCool.Middlewares;
using IsCool.Services;
using Microsoft.AspNetCore.Mvc;

namespace IsCool.Controller
{
    [ApiController]
    [Route("api/users")]
    public class UserController : IsCoolController
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [RequireProfileFilter]
        public async Task<IActionResult> Me()
        {
            return Ok(Result<UserDTO>.Success(CurrentUser!));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var result = await _userService.Login(login.Email, login.Password);
            return Ok(Result<string>.Success(result));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            await _userService.Register(
                register.Email,
                register.Name,
                register.UserName,
                register.StudentOf,
                register.Language,
                register.Password
            );
            return Created();
        }

        [HttpPost("forgot-password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            await _userService.RequestChangePassword(email);
            return Ok(Result<bool>.Success(true));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ForgotPasswordDTO dto)
        {
            await _userService.ChangePassword(dto.Email, dto.Otp, dto.NewPassword);
            return Ok(Result<bool>.Success(true));
        }
    }
}