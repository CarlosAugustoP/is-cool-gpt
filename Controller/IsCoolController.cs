using IsCool.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IsCool.Controller
{
    public class IsCoolController : ControllerBase
    {
        public UserDTO? CurrentUser => HttpContext.Items["User"] as UserDTO;
    }
}