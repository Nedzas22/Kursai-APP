using System.ComponentModel.DataAnnotations;

namespace Kursai.Api.DTOs
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateRatingDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Score { get; set; }

        [MaxLength(1000, ErrorMessage = "Review cannot exceed 1000 characters")]
        public string? Review { get; set; }
    }

    public class UpdateRatingDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Score { get; set; }

        [MaxLength(1000, ErrorMessage = "Review cannot exceed 1000 characters")]
        public string? Review { get; set; }
    }

    public class CourseRatingStatsDto
    {
        public int CourseId { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
    }
}
