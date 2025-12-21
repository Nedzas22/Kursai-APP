using System.ComponentModel.DataAnnotations;

namespace Kursai.Api.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? AttachmentFileName { get; set; }
        public string? AttachmentFileType { get; set; }
        public long? AttachmentFileSize { get; set; }
        public string? AttachmentFileUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Ratings
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

    public class CreateCourseDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public string? AttachmentFileName { get; set; }
        public string? AttachmentFileType { get; set; }
        public long? AttachmentFileSize { get; set; }
        public string? AttachmentFileUrl { get; set; }
    }

    public class UpdateCourseDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public string? AttachmentFileName { get; set; }
        public string? AttachmentFileType { get; set; }
        public long? AttachmentFileSize { get; set; }
        public string? AttachmentFileUrl { get; set; }
    }
}
