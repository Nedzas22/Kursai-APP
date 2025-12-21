namespace Kursai.maui.Models
{
    public class Rating
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

    public class CourseRatingStats
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
