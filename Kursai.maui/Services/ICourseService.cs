using Kursai.maui.Models;

namespace Kursai.maui.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<List<Course>> GetMyCoursesAsync(int userId);
        Task<List<Course>> GetFavoriteCoursesAsync(int userId);
        Task<List<Course>> GetPurchasedCoursesAsync(int userId);
        Task<Course> GetCourseByIdAsync(int id);
        Task<bool> AddCourseAsync(Course course);
        Task<bool> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);
        Task<bool> AddToFavoritesAsync(int userId, int courseId);
        Task<bool> RemoveFromFavoritesAsync(int userId, int courseId);
        Task<bool> IsFavoriteAsync(int userId, int courseId);
        Task<bool> PurchaseCourseAsync(int userId, int courseId);
    }
}