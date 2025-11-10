using System.Security.Cryptography;
using IsCool.Application;
using IsCool.Auth;
using IsCool.DB;
using IsCool.DTO;
using IsCool.Exceptions;
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
            var user = _db.Users.FirstOrDefault(u => u.Email == email) ?? throw new NotFoundException("Invalid credentials. Please try again.");
            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);
            return isPasswordValid ? _jwt.GenerateToken(user) : throw new NotFoundException("Invalid credentials. Please try again.");
        }
        public async Task<bool> Register(string email, string name, string username, string studentOf)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                throw new ConflictException("Email is already registered.");
            }
            var passwordHash = PasswordHasher.HashPassword("defaultPassword123");
            var newUser = new Models.User(name, username, email, passwordHash, studentOf);
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
            return true;
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
    }
}