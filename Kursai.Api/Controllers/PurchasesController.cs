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
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetPurchases()
        {
            var userId = GetUserId();

            var purchases = await _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.Course)
                .ThenInclude(c => c!.Seller)
                .OrderByDescending(p => p.PurchaseDate)
                .Select(p => new CourseDto
                {
                    Id = p.Course!.Id,
                    Title = p.Course.Title,
                    Description = p.Course.Description,
                    Price = p.Course.Price,
                    SellerId = p.Course.SellerId,
                    SellerName = p.Course.Seller!.Username,
                    Category = p.Course.Category,
                    ImageUrl = p.Course.ImageUrl,
                    CreatedAt = p.Course.CreatedAt
                })
                .ToListAsync();

            return Ok(purchases);
        }

        [HttpPost("{courseId}")]
        public async Task<ActionResult> PurchaseCourse(int courseId)
        {
            var userId = GetUserId();

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound(new { message = "Course not found" });
            }

            var exists = await _context.Purchases
                .AnyAsync(p => p.UserId == userId && p.CourseId == courseId);

            if (exists)
            {
                return BadRequest(new { message = "Course already purchased" });
            }

            var purchase = new Purchase
            {
                UserId = userId,
                CourseId = courseId,
                Price = course.Price,
                PurchaseDate = DateTime.UtcNow
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course purchased successfully" });
        }
    }
}
