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
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetFavorites()
        {
            var userId = GetUserId();

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Course)
                .ThenInclude(c => c!.Seller)
                .OrderByDescending(f => f.AddedDate)
                .Select(f => new CourseDto
                {
                    Id = f.Course!.Id,
                    Title = f.Course.Title,
                    Description = f.Course.Description,
                    Price = f.Course.Price,
                    SellerId = f.Course.SellerId,
                    SellerName = f.Course.Seller!.Username,
                    Category = f.Course.Category,
                    ImageUrl = f.Course.ImageUrl,
                    CreatedAt = f.Course.CreatedAt
                })
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpPost("{courseId}")]
        public async Task<ActionResult> AddFavorite(int courseId)
        {
            var userId = GetUserId();

            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
            {
                return NotFound(new { message = "Course not found" });
            }

            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.CourseId == courseId);

            if (exists)
            {
                return BadRequest(new { message = "Course already in favorites" });
            }

            var favorite = new Favorite
            {
                UserId = userId,
                CourseId = courseId,
                AddedDate = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Added to favorites" });
        }

        [HttpDelete("{courseId}")]
        public async Task<ActionResult> RemoveFavorite(int courseId)
        {
            var userId = GetUserId();

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CourseId == courseId);

            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("check/{courseId}")]
        public async Task<ActionResult<bool>> CheckFavorite(int courseId)
        {
            var userId = GetUserId();

            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.CourseId == courseId);

            return Ok(new { isFavorite = exists });
        }
    }
}
