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
using Microsoft.OpenApi.Models;
using Resumai.Services.Application;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

// ============================
// 🔧 CONFIGURAÇÕES DE SERVIÇOS
// ============================
// ============================
// 🐛 DEBUG MERDAZINHA (TEMPORÁRIO)
// ============================
// Verifica se o arquivo appsettings.json existe no diretório raiz do app.
Console.WriteLine("_________Verifing_________");
var appSettingsPath = Path.Combine(builder.Environment.ContentRootPath, "appsettings.json");
var fileExists = File.Exists(appSettingsPath);
Console.WriteLine("_________Verifing_________");

if (fileExists)
{
    Console.WriteLine($"[DEBUG-FILE] appsettings.json: ENCONTRADO em {appSettingsPath}");
}
else
{
    Console.WriteLine($"[DEBUG-FILE] appsettings.json: FALHA - NÃO ENCONTRADO em {appSettingsPath}");
}
// ============================
// FIM DO DEBUG MERDAZINHA
// ============================

// Conexão com o banco
builder.Services.AddPostgresConnection(builder.Configuration);

// AutoMapper
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://iscool-your-smart-learning-assistan.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
// Controllers
builder.Services.AddControllers();

// JWT Config
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

// Serviços
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<OpenAiService>();

builder.Services.AddSingleton(
    sp => new JwtService(sp.GetRequiredService<IOptions<JwtSettings>>())
);
builder.Services.AddSingleton(
    sp => new EmailService("SMPTKEY")
);

// Cache
builder.Services.AddFusionCache().AsHybridCache();

// ============================
// 🔐 AUTENTICAÇÃO E AUTORIZAÇÃO
// ============================

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings!.Secret);
Console.WriteLine("JWT Secret length: {Length} bytes", key.Length);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
   options.TokenValidationParameters = new TokenValidationParameters
   {
       ValidateIssuer = true,
       ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ============================
// 📘 SWAGGER
// ============================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IsCool API",
        Version = "v1",
        Description = "API do projeto IsCool"
    });

    // 🔒 Configuração do esquema de segurança JWT (para o cadeado)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

// ============================
// 🚀 BUILD E PIPELINE
// ============================

var app = builder.Build();

// Middlewares personalizados
app.UseMiddleware<IsCool.Middlewares.CatchExceptionMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IsCool API V1");
    c.RoutePrefix = string.Empty;
    c.DocumentTitle = "IsCool API Documentation";
    c.EnablePersistAuthorization();
    c.OAuthClientId("swagger-ui");
    c.OAuthUsePkce();
    c.OAuthScopeSeparator(" ");
    c.OAuthAppName("IsCool API - Swagger");
});

// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IsCool.Middlewares.UserValidationMiddleware>();
app.UseCors();
// Controllers
app.MapControllers();

app.Run();
