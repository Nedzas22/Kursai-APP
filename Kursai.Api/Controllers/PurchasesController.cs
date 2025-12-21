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
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public PurchasesController(ApplicationDbContext context, IEmailService emailService)
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
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetPurchases()
        {
            var userId = GetUserId();

            var purchases = await _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.Course)
                .ThenInclude(c => c!.Seller)
                .Include(p => p.Course)
                .ThenInclude(c => c!.Ratings)
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
                    AttachmentFileName = p.Course.AttachmentFileName,
                    AttachmentFileType = p.Course.AttachmentFileType,
                    AttachmentFileSize = p.Course.AttachmentFileSize,
                    AttachmentFileUrl = p.Course.AttachmentFileUrl,
                    CreatedAt = p.Course.CreatedAt,
                    AverageRating = p.Course.Ratings.Any() ? p.Course.Ratings.Average(r => r.Score) : 0,
                    TotalRatings = p.Course.Ratings.Count
                })
                .ToListAsync();

            return Ok(purchases);
        }

        [HttpPost("{courseId}")]
        public async Task<ActionResult> PurchaseCourse(int courseId)
        {
            var userId = GetUserId();

            var course = await _context.Courses
                .Include(c => c.Seller)
                .FirstOrDefaultAsync(c => c.Id == courseId);
                
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

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
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

            // Send purchase confirmation email
            try
            {
                await _emailService.SendPurchaseConfirmationAsync(
                    user.Email,
                    user.Username,
                    course.Title,
                    course.Price,
                    course.AttachmentFileUrl
                );
            }
            catch (Exception ex)
            {
                // Log error but don't fail the purchase
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }

            return Ok(new { message = "Course purchased successfully" });
        }
    }
}
