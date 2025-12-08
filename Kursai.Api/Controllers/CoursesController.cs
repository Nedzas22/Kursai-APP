using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kursai.Api.Data;
using Kursai.Api.DTOs;
using Kursai.Api.Models;
using System.Security.Claims;

namespace Kursai.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
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
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt
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
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt
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
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt
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
                    ImageUrl = dto.ImageUrl ?? "dotnet_bot.png",
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
                    ImageUrl = course.ImageUrl,
                    CreatedAt = course.CreatedAt
                };

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
            course.ImageUrl = dto.ImageUrl;

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
                ImageUrl = course.ImageUrl,
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
