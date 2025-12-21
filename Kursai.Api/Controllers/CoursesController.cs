using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kursai.Api.Data;
using Kursai.Api.DTOs;
using Kursai.Api.Models;
using Kursai.Api.Services;
using System.Security.Claims;

namespace Kursai.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public CoursesController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
        {
            var courses = await _context.Courses
                .Include(c => c.Seller)
                .Include(c => c.Ratings)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,
                    SellerId = c.SellerId,
                    SellerName = c.Seller!.Username,
                    Category = c.Category,
                    AttachmentFileName = c.AttachmentFileName,
                    AttachmentFileType = c.AttachmentFileType,
                    AttachmentFileUrl = c.AttachmentFileUrl,
                    AttachmentFileSize = c.AttachmentFileSize,
                    CreatedAt = c.CreatedAt,
                    AverageRating = c.Ratings.Any() ? c.Ratings.Average(r => r.Score) : 0,
                    TotalRatings = c.Ratings.Count
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDto>> GetById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Seller)
                .Include(c => c.Ratings)
                .Where(c => c.Id == id)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,
                    SellerId = c.SellerId,
                    SellerName = c.Seller!.Username,
                    Category = c.Category,
                    AttachmentFileName = c.AttachmentFileName,
                    AttachmentFileType = c.AttachmentFileType,
                    AttachmentFileUrl = c.AttachmentFileUrl,
                    AttachmentFileSize = c.AttachmentFileSize,
                    CreatedAt = c.CreatedAt,
                    // Ratings
                    AverageRating = c.Ratings.Any() ? c.Ratings.Average(r => r.Score) : 0,
                    TotalRatings = c.Ratings.Count
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        [HttpGet("my-courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetMyCourses()
        {
            var userId = GetUserId();

            var courses = await _context.Courses
                .Where(c => c.SellerId == userId)
                .Include(c => c.Seller)
                .Include(c => c.Ratings)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,
                    SellerId = c.SellerId,
                    SellerName = c.Seller!.Username,
                    Category = c.Category,
                    AttachmentFileName = c.AttachmentFileName,
                    AttachmentFileType = c.AttachmentFileType,
                    AttachmentFileUrl = c.AttachmentFileUrl,
                    AttachmentFileSize = c.AttachmentFileSize,
                    CreatedAt = c.CreatedAt,
                    // Ratings
                    AverageRating = c.Ratings.Any() ? c.Ratings.Average(r => r.Score) : 0,
                    TotalRatings = c.Ratings.Count
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
        {
            try
            {
                var userId = GetUserId();

                var course = new Course
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price,
                    SellerId = userId,
                    Category = dto.Category,
                    AttachmentFileName = dto.AttachmentFileName,
                    AttachmentFileType = dto.AttachmentFileType,
                    AttachmentFileUrl = dto.AttachmentFileUrl,
                    AttachmentFileSize = dto.AttachmentFileSize,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                var seller = await _context.Users.FindAsync(userId);

                var courseDto = new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Price = course.Price,
                    SellerId = course.SellerId,
                    SellerName = seller!.Username,
                    Category = course.Category,
                    AttachmentFileName = course.AttachmentFileName,
                    AttachmentFileType = course.AttachmentFileType,
                    AttachmentFileUrl = course.AttachmentFileUrl,
                    AttachmentFileSize = course.AttachmentFileSize,
                    CreatedAt = course.CreatedAt
                };

                // Send new course notification email
                try
                {
                    await _emailService.SendNewCourseNotificationAsync(
                        seller.Email,
                        seller.Username,
                        course.Title
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }

                return CreatedAtAction(nameof(GetById), new { id = course.Id }, courseDto);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error creating course: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return StatusCode(500, new { message = "Failed to create course", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> Update(int id, [FromBody] UpdateCourseDto dto)
        {
            var userId = GetUserId();
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            if (course.SellerId != userId)
            {
                return Forbid();
            }

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Price = dto.Price;
            course.Category = dto.Category;
            course.AttachmentFileName = dto.AttachmentFileName;
            course.AttachmentFileType = dto.AttachmentFileType;
            course.AttachmentFileUrl = dto.AttachmentFileUrl;
            course.AttachmentFileSize = dto.AttachmentFileSize;

            await _context.SaveChangesAsync();

            var seller = await _context.Users.FindAsync(userId);

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                SellerId = course.SellerId,
                SellerName = seller!.Username,
                Category = course.Category,
                AttachmentFileName = course.AttachmentFileName,
                AttachmentFileType = course.AttachmentFileType,
                AttachmentFileUrl = course.AttachmentFileUrl,
                AttachmentFileSize = course.AttachmentFileSize,
                CreatedAt = course.CreatedAt
            };

            return Ok(courseDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            if (course.SellerId != userId)
            {
                return Forbid();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
