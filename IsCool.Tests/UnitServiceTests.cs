using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using IsCool.Services;
using IsCool.Exceptions;
using IsCool.Auth;
using IsCool.DB;
using IsCool.Models;
using IsCool.Application;
using ZiggyCreatures.Caching.Fusion;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Security.Cryptography; 

namespace IsCool.Tests
{
    // A classe AppDbContext precisa ser configurada no Program.cs para ser injetada.
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly AppDbContext _context;
        private readonly Mock<IFusionCache> _mockCache = new Mock<IFusionCache>();
        private readonly Mock<IJwtService> _mockJwt = new Mock<IJwtService>();
        private readonly Mock<IPasswordHasher> _mockHasher = new Mock<IPasswordHasher>();
        private readonly Mock<IEmailService> _mockEmail = new Mock<IEmailService>();

        private const string MOCK_HASH_SUCCESS = "HASHED_PASSWORD";
        private const string VALID_PASSWORD = "senha_valida";
        private const string INVALID_PASSWORD = "senha_invalida";

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);

            
            _mockHasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns(MOCK_HASH_SUCCESS);

            _mockHasher.Setup(h => h.VerifyPassword(VALID_PASSWORD, It.IsAny<string>())).Returns(true);
            
            _mockHasher.Setup(h => h.VerifyPassword(INVALID_PASSWORD, It.IsAny<string>())).Returns(false);
            _mockHasher.Setup(h => h.VerifyPassword(It.Is<string>(p => p != VALID_PASSWORD), It.IsAny<string>())).Returns(false);

            _mockJwt.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("MOCK_JWT_TOKEN");

            _userService = new UserService(_context, _mockCache.Object, _mockJwt.Object, _mockEmail.Object, _mockHasher.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact(DisplayName = "Login: Deve retornar token para credenciais válidas")]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            _context.Users.Add(new User("Test Name", "testuser", "test@iscool.com", MOCK_HASH_SUCCESS, "High School", Language.EN));
            await _context.SaveChangesAsync();
            
            var token = await _userService.Login("test@iscool.com", VALID_PASSWORD);

            Assert.Equal("MOCK_JWT_TOKEN", token);
            _mockJwt.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Once);
            _mockHasher.Verify(h => h.VerifyPassword(VALID_PASSWORD, MOCK_HASH_SUCCESS), Times.Once); 
        }

        [Fact(DisplayName = "Login: Deve lançar exceção NotFound para senha inválida")]
        public async Task Login_InvalidPassword_ThrowsNotFoundException()
        {
            // Arrange
            _context.Users.Add(new User("Test Name", "testuser", "test@iscool.com", MOCK_HASH_SUCCESS, "High School", Language.EN));
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _userService.Login("test@iscool.com", INVALID_PASSWORD)
            );
            _mockHasher.Verify(h => h.VerifyPassword(INVALID_PASSWORD, MOCK_HASH_SUCCESS), Times.Once); 
        }

        [Fact(DisplayName = "Registro: Deve adicionar novo usuário ao banco de dados e aplicar hash simulado")]
        public async Task Register_NewUser_AddsToDb()
        {
            var initialCount = _context.Users.Count();
            const string rawPassword = "secure_password";

            await _userService.Register("new@iscool.com", "New User", "newuser", "College", Language.PTBR, rawPassword);

            Assert.Equal(initialCount + 1, _context.Users.Count());
            var user = await _context.Users.FirstAsync(u => u.Email == "new@iscool.com");
            Assert.Equal(MOCK_HASH_SUCCESS, user.PasswordHash); 
            _mockHasher.Verify(h => h.HashPassword(rawPassword), Times.Once);
        }

        [Fact(DisplayName = "Registro: Deve lançar ConflictException se o email já existe")]
        public async Task Register_ExistingEmail_ThrowsConflictException()
        {
            // Arrange
            _context.Users.Add(new User("Old User", "olduser", "conflict@iscool.com", MOCK_HASH_SUCCESS, "High School", Language.EN));
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => 
                _userService.Register("conflict@iscool.com", "New User", "newuser", "College", Language.PTBR, "secure_password")
            );
        }
    
    }
}