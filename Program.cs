using IsCool.Application;
using IsCool.Auth;
using IsCool.DB;
using IsCool.DI;
using IsCool.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPostgresConnection(builder.Configuration);

builder.Services.AddAutoMapper(
    cfg => {},
    typeof(Program)
);

builder.Services.AddControllers();
builder.Services.AddScoped<UserService>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
builder.Services.AddSingleton(
    sp => new JwtService(sp.GetRequiredService<IOptions<JwtSettings>>())
);

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChatService>();

builder.Services.AddSingleton(
    sp => new EmailService("SMPTKEY")
);
builder.Services.AddFusionCache().AsHybridCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<IsCool.Middlewares.CatchExceptionMiddleware>();
app.UseMiddleware<IsCool.Middlewares.UserValidationMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IsCool API V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();
