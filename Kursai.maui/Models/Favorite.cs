namespace Kursai.maui.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime AddedDate { get; set; }
    }
}

