using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Kursai.Api.Controllers;
using Kursai.Api.Data;
using Kursai.Api.DTOs;
using Kursai.Api.Services;
using Kursai.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Kursai.Tests
{
    // ==================== AUTHENTICATION TESTS ====================
    public class AuthControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockJwtService = new Mock<IJwtService>();
            _controller = new AuthController(_context, _mockJwtService.Object);
        }

        [Fact]
        public async Task Test01_Register_WithValidData_ReturnsOkResult()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test123!"
            };

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("test-token");

            var result = await _controller.Register(registerDto);

            Assert.IsType<ActionResult<AuthResponseDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Test02_Register_WithDuplicateUsername_ReturnsBadRequest()
        {
            var existingUser = new User
            {
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var registerDto = new RegisterDto
            {
                Username = "existinguser",
                Email = "new@example.com",
                Password = "Test123!"
            };

            var result = await _controller.Register(registerDto);

            Assert.IsType<ActionResult<AuthResponseDto>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Test03_Login_WithValidCredentials_ReturnsOkResult()
        {
            var user = new User
            {
                Username = "loginuser",
                Email = "login@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Username = "loginuser",
                Password = "Test123!"
            };

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("test-token");

            var result = await _controller.Login(loginDto);

            Assert.IsType<ActionResult<AuthResponseDto>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Test04_Login_WithInvalidUsername_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto
            {
                Username = "nonexistent",
                Password = "Test123!"
            };

            var result = await _controller.Login(loginDto);

            Assert.IsType<ActionResult<AuthResponseDto>>(result);
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Test05_Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            var result = await _controller.Login(loginDto);

            Assert.IsType<ActionResult<AuthResponseDto>>(result);
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

    // ==================== COURSE TESTS ====================
    public class CoursesControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CoursesController _controller;
        private readonly User _testUser;
        private readonly Mock<IEmailService> _mockEmailService;

        public CoursesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            
            _testUser = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
            };
            _context.Users.Add(_testUser);
            _context.SaveChanges();

            _mockEmailService = new Mock<IEmailService>();
            _controller = new CoursesController(_context, _mockEmailService.Object);
            SetupControllerContext(_controller, _testUser.Id);
        }

        private void SetupControllerContext(ControllerBase controller, int userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task Test06_GetAll_ReturnsAllCourses()
        {
            var course1 = new Course { Title = "C# Basics", Description = "Learn C#", Price = 29.99m, Category = "Programming", SellerId = _testUser.Id };
            var course2 = new Course { Title = "Python Advanced", Description = "Advanced Python", Price = 49.99m, Category = "Programming", SellerId = _testUser.Id };
            _context.Courses.AddRange(course1, course2);
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var courses = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(okResult.Value);
            Assert.Equal(2, courses.Count());
        }

        [Fact]
        public async Task Test07_GetById_WithValidId_ReturnsCourse()
        {
            var course = new Course { Title = "Test Course", Description = "Test Description", Price = 19.99m, Category = "Testing", SellerId = _testUser.Id };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(course.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var courseDto = Assert.IsType<CourseDto>(okResult.Value);
            Assert.Equal("Test Course", courseDto.Title);
        }

        [Fact]
        public async Task Test08_GetById_WithInvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Test09_Create_WithValidData_ReturnsCreatedCourse()
        {
            var createDto = new CreateCourseDto
            {
                Title = "New Course",
                Description = "New Description",
                Price = 39.99m,
                Category = "Web Development"
            };

            var result = await _controller.Create(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var courseDto = Assert.IsType<CourseDto>(createdResult.Value);
            Assert.Equal("New Course", courseDto.Title);
        }

        [Fact]
        public async Task Test10_Update_ByOwner_UpdatesCourse()
        {
            var course = new Course { Title = "Original Title", Description = "Original Description", Price = 29.99m, Category = "Programming", SellerId = _testUser.Id };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateCourseDto { Title = "Updated Title", Description = "Updated Description", Price = 39.99m, Category = "Web Development" };

            var result = await _controller.Update(course.Id, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var courseDto = Assert.IsType<CourseDto>(okResult.Value);
            Assert.Equal("Updated Title", courseDto.Title);
        }

        [Fact]
        public async Task Test11_Delete_ByOwner_DeletesCourse()
        {
            var course = new Course { Title = "To Delete", Description = "Description", Price = 29.99m, Category = "Programming", SellerId = _testUser.Id };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(course.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(0, await _context.Courses.CountAsync());
        }

        [Fact]
        public async Task Test12_GetMyCourses_ReturnsOnlyUserCourses()
        {
            var otherUser = new User { Id = 2, Username = "otheruser", Email = "other@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _context.Users.Add(otherUser);

            var myCourse = new Course { Title = "My Course", Description = "My Description", Price = 29.99m, Category = "Programming", SellerId = _testUser.Id };
            var otherCourse = new Course { Title = "Other Course", Description = "Other Description", Price = 39.99m, Category = "Programming", SellerId = otherUser.Id };
            _context.Courses.AddRange(myCourse, otherCourse);
            await _context.SaveChangesAsync();

            var result = await _controller.GetMyCourses();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var courses = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(okResult.Value);
            Assert.Single(courses);
            Assert.Equal("My Course", courses.First().Title);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

    // ==================== FAVORITES TESTS ====================
    public class FavoritesControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly FavoritesController _controller;
        private readonly User _testUser;
        private readonly Course _testCourse;

        public FavoritesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            
            _testUser = new User { Id = 1, Username = "testuser", Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _context.Users.Add(_testUser);

            _testCourse = new Course { Id = 1, Title = "Test Course", Description = "Test Description", Price = 29.99m, Category = "Programming", SellerId = _testUser.Id };
            _context.Courses.Add(_testCourse);
            _context.SaveChanges();

            _controller = new FavoritesController(_context);
            SetupControllerContext(_controller, _testUser.Id);
        }

        private void SetupControllerContext(ControllerBase controller, int userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task Test13_AddFavorite_WithValidCourse_AddsToFavorites()
        {
            var result = await _controller.AddFavorite(_testCourse.Id);

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, await _context.Favorites.CountAsync());
        }

        [Fact]
        public async Task Test14_AddFavorite_DuplicateFavorite_ReturnsBadRequest()
        {
            await _controller.AddFavorite(_testCourse.Id);

            var result = await _controller.AddFavorite(_testCourse.Id);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test15_AddFavorite_NonExistentCourse_ReturnsNotFound()
        {
            var result = await _controller.AddFavorite(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Test16_GetFavorites_ReturnsUserFavorites()
        {
            var favorite = new Favorite { UserId = _testUser.Id, CourseId = _testCourse.Id, AddedDate = DateTime.UtcNow };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            var result = await _controller.GetFavorites();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var favorites = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(okResult.Value);
            Assert.Single(favorites);
        }

        [Fact]
        public async Task Test17_RemoveFavorite_ExistingFavorite_RemovesFromFavorites()
        {
            var favorite = new Favorite { UserId = _testUser.Id, CourseId = _testCourse.Id, AddedDate = DateTime.UtcNow };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            var result = await _controller.RemoveFavorite(_testCourse.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(0, await _context.Favorites.CountAsync());
        }

        [Fact]
        public async Task Test18_CheckFavorite_ExistingFavorite_ReturnsTrue()
        {
            var favorite = new Favorite { UserId = _testUser.Id, CourseId = _testCourse.Id, AddedDate = DateTime.UtcNow };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            var result = await _controller.CheckFavorite(_testCourse.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

    // ==================== JWT SERVICE TESTS ====================
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public JwtServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x["SecretKey"]).Returns("ThisIsAVerySecureSecretKeyForTestingPurposesOnly12345678");
            jwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSection.Setup(x => x["Audience"]).Returns("TestAudience");
            jwtSection.Setup(x => x["ExpirationHours"]).Returns("24");
            
            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);
            
            _jwtService = new JwtService(_mockConfiguration.Object);
        }

        [Fact]
        public void Test19_GenerateToken_WithValidUser_ReturnsToken()
        {
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };

            var token = _jwtService.GenerateToken(user);

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void Test20_GenerateToken_TokenContainsUserClaims()
        {
            var user = new User { Id = 123, Username = "testuser", Email = "test@example.com" };

            var token = _jwtService.GenerateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");

            Assert.NotNull(userIdClaim);
            Assert.Equal("123", userIdClaim.Value);
        }
    }
}
