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
        public DateTime CreatedAt { get; set; }
    }
}}