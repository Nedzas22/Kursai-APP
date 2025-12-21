using Kursai.maui.Models;

namespace Kursai.maui.Services
{
    public class CourseService : ICourseService
    {
        private readonly List<Course> _courses = new();
        private readonly List<Favorite> _favorites = new();
        private readonly List<Purchase> _purchases = new();
        private readonly List<Rating> _ratings = new();

        public CourseService()
        {
            // Demo data
            _courses.AddRange(new[]
            {
                new Course
                {
                    Id = 1,
                    Title = "C# Complete Course",
                    Description = "Learn C# from basics to advanced",
                    Price = 49.99m,
                    SellerId = 1,
                    SellerName = "demo",
                    Category = "Programming",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    AverageRating = 4.5,
                    TotalRatings = 12
                },
                new Course
                {
                    Id = 2,
                    Title = "Java Fundamentals",
                    Description = "Master Java programming",
                    Price = 39.99m,
                    SellerId = 1,
                    SellerName = "demo",
                    Category = "Programming",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    AverageRating = 4.2,
                    TotalRatings = 8
                },
                new Course
                {
                    Id = 3,
                    Title = "Python for Beginners",
                    Description = "Start your Python journey",
                    Price = 29.99m,
                    SellerId = 1,
                    SellerName = "demo",
                    Category = "Programming",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    AverageRating = 4.8,
                    TotalRatings = 25
                }
            });
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            await Task.Delay(300); // Simulate API call
            return _courses.OrderByDescending(c => c.CreatedAt).ToList();
        }

        public async Task<List<Course>> GetMyCoursesAsync(int userId)
        {
            await Task.Delay(300);
            return _courses.Where(c => c.SellerId == userId).ToList();
        }

        public async Task<List<Course>> GetFavoriteCoursesAsync(int userId)
        {
            await Task.Delay(300);
            var favoriteCourseIds = _favorites.Where(f => f.UserId == userId).Select(f => f.CourseId);
            return _courses.Where(c => favoriteCourseIds.Contains(c.Id)).ToList();
        }

        public async Task<List<Course>> GetPurchasedCoursesAsync(int userId)
        {
            await Task.Delay(300);
            var purchasedCourseIds = _purchases.Where(p => p.UserId == userId).Select(p => p.CourseId);
            return _courses.Where(c => purchasedCourseIds.Contains(c.Id)).ToList();
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            await Task.Delay(300);
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        public async Task<bool> AddCourseAsync(Course course)
        {
            await Task.Delay(300);
            course.Id = _courses.Count > 0 ? _courses.Max(c => c.Id) + 1 : 1;
            course.CreatedAt = DateTime.Now;
            _courses.Add(course);
            return true;
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            await Task.Delay(300);
            var existing = _courses.FirstOrDefault(c => c.Id == course.Id);
            if (existing == null) return false;

            existing.Title = course.Title;
            existing.Description = course.Description;
            existing.Price = course.Price;
            existing.Category = course.Category;
            existing.AttachmentFileName = course.AttachmentFileName;
            existing.AttachmentFileType = course.AttachmentFileType;
            existing.AttachmentFileUrl = course.AttachmentFileUrl;
            existing.AttachmentFileSize = course.AttachmentFileSize;
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            await Task.Delay(300);
            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return false;

            _courses.Remove(course);
            return true;
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int courseId)
        {
            await Task.Delay(300);
            
            if (_favorites.Any(f => f.UserId == userId && f.CourseId == courseId))
                return false;

            var course = await GetCourseByIdAsync(courseId);
            if (course == null) return false;

            var favorite = new Favorite
            {
                Id = _favorites.Count > 0 ? _favorites.Max(f => f.Id) + 1 : 1,
                UserId = userId,
                CourseId = courseId,
                AddedDate = DateTime.Now
            };

            _favorites.Add(favorite);
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int courseId)
        {
            await Task.Delay(300);
            
            var favorite = _favorites.FirstOrDefault(f => f.UserId == userId && f.CourseId == courseId);
            if (favorite == null) return false;

            _favorites.Remove(favorite);
            return true;
        }

        public async Task<bool> IsFavoriteAsync(int userId, int courseId)
        {
            await Task.Delay(100);
            return _favorites.Any(f => f.UserId == userId && f.CourseId == courseId);
        }

        public async Task<bool> PurchaseCourseAsync(int userId, int courseId)
        {
            await Task.Delay(300);
            
            if (_purchases.Any(p => p.UserId == userId && p.CourseId == courseId))
                return false; // Already purchased

            var course = await GetCourseByIdAsync(courseId);
            if (course == null) return false;

            var purchase = new Purchase
            {
                Id = _purchases.Count > 0 ? _purchases.Max(p => p.Id) + 1 : 1,
                UserId = userId,
                CourseId = courseId,
                PurchaseDate = DateTime.Now,
                Price = course.Price
            };

            _purchases.Add(purchase);
            return true;
        }

        // Rating methods
        public async Task<Rating?> GetUserRatingAsync(int userId, int courseId)
        {
            await Task.Delay(100);
            return _ratings.FirstOrDefault(r => r.UserId == userId && r.CourseId == courseId);
        }

        public async Task<List<Rating>> GetCourseRatingsAsync(int courseId)
        {
            await Task.Delay(300);
            return _ratings.Where(r => r.CourseId == courseId).OrderByDescending(r => r.CreatedAt).ToList();
        }

        public async Task<bool> SubmitRatingAsync(int userId, int courseId, int score, string? review)
        {
            await Task.Delay(300);

            if (_ratings.Any(r => r.UserId == userId && r.CourseId == courseId))
                return false; // Already rated

            var rating = new Rating
            {
                Id = _ratings.Count > 0 ? _ratings.Max(r => r.Id) + 1 : 1,
                UserId = userId,
                CourseId = courseId,
                Score = score,
                Review = review,
                Username = "User", // In real app, get from user service
                CreatedAt = DateTime.Now
            };

            _ratings.Add(rating);

            // Update course average rating
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course != null)
            {
                var courseRatings = _ratings.Where(r => r.CourseId == courseId).ToList();
                course.AverageRating = courseRatings.Average(r => r.Score);
                course.TotalRatings = courseRatings.Count;
            }

            return true;
        }

        public async Task<bool> UpdateRatingAsync(int ratingId, int score, string? review)
        {
            await Task.Delay(300);

            var rating = _ratings.FirstOrDefault(r => r.Id == ratingId);
            if (rating == null) return false;

            rating.Score = score;
            rating.Review = review;
            rating.UpdatedAt = DateTime.Now;

            // Update course average rating
            var course = _courses.FirstOrDefault(c => c.Id == rating.CourseId);
            if (course != null)
            {
                var courseRatings = _ratings.Where(r => r.CourseId == rating.CourseId).ToList();
                course.AverageRating = courseRatings.Average(r => r.Score);
            }

            return true;
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            await Task.Delay(300);

            var rating = _ratings.FirstOrDefault(r => r.Id == ratingId);
            if (rating == null) return false;

            _ratings.Remove(rating);

            // Update course average rating
            var course = _courses.FirstOrDefault(c => c.Id == rating.CourseId);
            if (course != null)
            {
                var courseRatings = _ratings.Where(r => r.CourseId == rating.CourseId).ToList();
                course.AverageRating = courseRatings.Any() ? courseRatings.Average(r => r.Score) : 0;
                course.TotalRatings = courseRatings.Count;
            }

            return true;
        }
    }
}