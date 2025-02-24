using ConstructionProjectManagement.Controllers;
using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ConstructionProjectManagement.Tests
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly ConstructionDbContext _context;
        private readonly Moq.Mock<Microsoft.Extensions.Configuration.IConfiguration> _configurationMock;

        public AuthControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConstructionDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDatabase")
                          .Options;

            _context = new ConstructionDbContext(options); 
            _configurationMock = new Moq.Mock<Microsoft.Extensions.Configuration.IConfiguration>();

            _configurationMock.SetupGet(x => x["Jwt:Key"]).Returns("your_new_secret_key_which_is_at_least_16_chars");
            _configurationMock.SetupGet(x => x["Jwt:Issuer"]).Returns("grafikIssuer");
            _configurationMock.SetupGet(x => x["Jwt:Audience"]).Returns("grafikAudience");

            _controller = new AuthController(_context, _configurationMock.Object);

            // Seed the database
            _context.Users.Add(new User
            {
                Email = "test@admin.com",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123")
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task Login_ValidUser_ReturnsToken()
        {
            var loginModel = new LoginModel
            {
                Email = "test@admin.com",
                Password = "admin123"
            };

            var result = await _controller.Login(loginModel);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_InvalidUser_ReturnsUnauthorized()
        {
            var loginModel = new LoginModel
            {
                Email = "invalid@example.com",
                Password = "invalid"
            };

            var result = await _controller.Login(loginModel);

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}