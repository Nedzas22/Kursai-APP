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
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public RatingsController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        // GET: api/ratings/course/{courseId}
        [HttpGet("course/{courseId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetCourseRatings(int courseId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.CourseId == courseId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    CourseId = r.CourseId,
                    UserId = r.UserId,
                    Username = r.User!.Username,
                    Score = r.Score,
                    Review = r.Review,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();

            return Ok(ratings);
        }

        // GET: api/ratings/course/{courseId}/stats
        [HttpGet("course/{courseId}/stats")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseRatingStatsDto>> GetCourseRatingStats(int courseId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.CourseId == courseId)
                .ToListAsync();

            if (!ratings.Any())
            {
                return Ok(new CourseRatingStatsDto
                {
                    CourseId = courseId,
                    AverageRating = 0,
                    TotalRatings = 0
                });
            }

            var stats = new CourseRatingStatsDto
            {
                CourseId = courseId,
                AverageRating = ratings.Average(r => r.Score),
                TotalRatings = ratings.Count,
                FiveStars = ratings.Count(r => r.Score == 5),
                FourStars = ratings.Count(r => r.Score == 4),
                ThreeStars = ratings.Count(r => r.Score == 3),
                TwoStars = ratings.Count(r => r.Score == 2),
                OneStar = ratings.Count(r => r.Score == 1)
            };

            return Ok(stats);
        }

        // GET: api/ratings/course/{courseId}/user
        [HttpGet("course/{courseId}/user")]
        public async Task<ActionResult<RatingDto>> GetUserRatingForCourse(int courseId)
        {
            var userId = GetUserId();

            var rating = await _context.Ratings
                .Where(r => r.CourseId == courseId && r.UserId == userId)
                .Include(r => r.User)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    CourseId = r.CourseId,
                    UserId = r.UserId,
                    Username = r.User!.Username,
                    Score = r.Score,
                    Review = r.Review,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        // POST: api/ratings/course/{courseId}
        [HttpPost("course/{courseId}")]
        public async Task<ActionResult<RatingDto>> CreateRating(int courseId, [FromBody] CreateRatingDto dto)
        {
            var userId = GetUserId();

            // Check if course exists
            var course = await _context.Courses
                .Include(c => c.Seller)
                .FirstOrDefaultAsync(c => c.Id == courseId);
                
            if (course == null)
            {
                return NotFound(new { message = "Course not found" });
            }

            // Check if user has purchased the course or owns it
            var hasPurchased = await _context.Purchases
                .AnyAsync(p => p.CourseId == courseId && p.UserId == userId);
            var isOwner = course.SellerId == userId;

            if (!hasPurchased && !isOwner)
            {
                return BadRequest(new { message = "You must purchase the course before rating it" });
            }

            // Check if user already rated this course
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.CourseId == courseId && r.UserId == userId);

            if (existingRating != null)
            {
                return BadRequest(new { message = "You have already rated this course. Use PUT to update your rating." });
            }

            var rating = new Rating
            {
                CourseId = courseId,
                UserId = userId,
                Score = dto.Score,
                Review = dto.Review,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            var ratingDto = new RatingDto
            {
                Id = rating.Id,
                CourseId = rating.CourseId,
                UserId = rating.UserId,
                Username = user!.Username,
                Score = rating.Score,
                Review = rating.Review,
                CreatedAt = rating.CreatedAt
            };

            // Send rating notification email to course seller
            try
            {
                await _emailService.SendRatingReceivedAsync(
                    course.Seller!.Email,
                    course.Title,
                    rating.Score,
                    rating.Review
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetUserRatingForCourse), new { courseId }, ratingDto);
        }

        // PUT: api/ratings/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RatingDto>> UpdateRating(int id, [FromBody] UpdateRatingDto dto)
        {
            var userId = GetUserId();

            var rating = await _context.Ratings
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rating == null)
            {
                return NotFound();
            }

            if (rating.UserId != userId)
            {
                return Forbid();
            }

            rating.Score = dto.Score;
            rating.Review = dto.Review;
            rating.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var ratingDto = new RatingDto
            {
                Id = rating.Id,
                CourseId = rating.CourseId,
                UserId = rating.UserId,
                Username = rating.User!.Username,
                Score = rating.Score,
                Review = rating.Review,
                CreatedAt = rating.CreatedAt,
                UpdatedAt = rating.UpdatedAt
            };

            return Ok(ratingDto);
        }

        // DELETE: api/ratings/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRating(int id)
        {
            var userId = GetUserId();

            var rating = await _context.Ratings.FindAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            if (rating.UserId != userId)
            {
                return Forbid();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
