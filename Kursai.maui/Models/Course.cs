namespace Kursai.maui.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public string Category { get; set; }
        public string? AttachmentFileName { get; set; }
        public string? AttachmentFileType { get; set; }
        public string? AttachmentFileUrl { get; set; }
        public long? AttachmentFileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Ratings
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
}