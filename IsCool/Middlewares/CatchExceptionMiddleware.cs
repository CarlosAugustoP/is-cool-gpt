using IsCool.Abstractions;
using IsCool.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace IsCool.Middlewares
{
    public class CatchExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CatchExceptionMiddleware> _logger;

        public CatchExceptionMiddleware(RequestDelegate next, ILogger<CatchExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        private static Result<string> Fail(string err) => Result<string>.Failure(err);

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException nfEx)
            {
                _logger.LogWarning(nfEx, "Resource not found.");

                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(Fail(nfEx.Message));
            }
            catch (DomainException dEx)
            {
                _logger.LogWarning(dEx, "Domain error occurred.");

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(Fail(dEx.Message));
            }
            catch (ConflictException cEx)
            {
                _logger.LogWarning(cEx, "Conflict occurred.");

                context.Response.StatusCode = 409;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = cEx.Message
                };

                await context.Response.WriteAsJsonAsync(Fail(cEx.Message));
            }
            catch (UnauthorizedAccessException uEx)
            {
                _logger.LogWarning(uEx, "Unauthorized access.");

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(Fail(uEx.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(Fail("An unexpected error occurred. Please try again later."));
            }
        }
        
    }
}