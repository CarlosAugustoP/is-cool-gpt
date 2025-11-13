
﻿using System.Security.Claims;
using IsCool.DB;
using IsCool.DTO;
using Microsoft.AspNetCore.Http;

namespace IsCool.Middlewares
{
    public class UserValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            Console.WriteLine("Invoking UserValidationMiddleware...");
            if (context.User.Identity?.IsAuthenticated == true)
            {

                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {

                    var userDTO = new UserDTO
                    {
                        Email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                        Id = Guid.Parse(userId),
                        Name = context.User.FindFirst(ClaimTypes.Name)?.Value ?? "",
                    };

                    context.Items["User"] = userDTO;
                }
            }
            await _next(context);
        }
    }
}
