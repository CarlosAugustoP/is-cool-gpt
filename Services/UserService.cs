using System.Security.Cryptography;
using IsCool.Application;
using IsCool.Auth;
using IsCool.DB;
using IsCool.Exceptions;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

namespace IsCool.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        private readonly IFusionCache _cache;
        private readonly JwtService _jwt;
        private readonly EmailService _emailService;
        public UserService(AppDbContext db, IFusionCache cache, JwtService jwt, EmailService emailService)
        {
            _db = db;
            _cache = cache;
            _jwt = jwt;
            _emailService = emailService;
        }
        public async Task<string> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email) 
                ?? throw new NotFoundException("Invalid credentials. Please try again.");
            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);
            return isPasswordValid ? _jwt.GenerateToken(user) : throw new NotFoundException("Invalid credentials. Please try again.");
        }
        public async Task Register(string email, string name, string username, string studentOf, Models.Language preferredLanguage, string password)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Email == email || u.Username == username);
            if (existingUser != null)
            {
                throw new ConflictException("Email is already registered.");
            }
            var passwordHash = PasswordHasher.HashPassword(password);
            var newUser = new Models.User(name, username, email, passwordHash, studentOf, preferredLanguage);
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> RequestChangePassword(string email)
        {
            if (!_db.Users.Any(u => u.Email == email))
            {
                //Security 
                return true;
            }
            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            await _cache.SetAsync($"PasswordResetOTP_{email}", otp, TimeSpan.FromMinutes(15));
            await _emailService.SendEmail(email, $"Hello from IsCool AI! Your OTP is {otp}", "Password Reset OTP");
            return true;
        }

        public async Task<bool> ChangePassword(string email, string otp, string newPassword)
        {
            var cachedOtp = await _cache.GetOrDefaultAsync<string>($"PasswordResetOTP_{email}");
            if (cachedOtp == null || cachedOtp != otp)
            {
                throw new DomainException("Invalid or expired OTP.");
            }
            var user = _db.Users.FirstOrDefault(u => u.Email == email) ?? throw new NotFoundException("User not found.");
            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            await _db.SaveChangesAsync();
            await _cache.RemoveAsync($"PasswordResetOTP_{email}");
            return true;
        }
    }
}