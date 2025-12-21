using Kursai.maui.Models;
using Kursai.maui.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Kursai.maui.Services
{
    public class ApiCourseService : ICourseService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ApiCourseService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/courses");
                response.EnsureSuccessStatusCode();
                
                var courses = await response.Content.ReadFromJsonAsync<List<Course>>();
                return courses ?? new List<Course>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting all courses: {ex.Message}");
                return new List<Course>();
            }
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/courses/{id}");
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<Course>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting course: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Course>> GetMyCoursesAsync(int userId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/courses/my-courses");
                response.EnsureSuccessStatusCode();
                
                var courses = await response.Content.ReadFromJsonAsync<List<Course>>();
                return courses ?? new List<Course>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting my courses: {ex.Message}");
                return new List<Course>();
            }
        }

        public async Task<List<Course>> GetFavoriteCoursesAsync(int userId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/favorites");
                response.EnsureSuccessStatusCode();
                
                var courses = await response.Content.ReadFromJsonAsync<List<Course>>();
                return courses ?? new List<Course>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting favorites: {ex.Message}");
                return new List<Course>();
            }
        }

        public async Task<List<Course>> GetPurchasedCoursesAsync(int userId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/purchases");
                response.EnsureSuccessStatusCode();
                
                var courses = await response.Content.ReadFromJsonAsync<List<Course>>();
                return courses ?? new List<Course>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting purchases: {ex.Message}");
                return new List<Course>();
            }
        }

        public async Task<bool> AddCourseAsync(Course course)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var createDto = new
                {
                    title = course.Title,
                    description = course.Description,
                    price = course.Price,
                    category = course.Category,
                    attachmentFileName = course.AttachmentFileName,
                    attachmentFileType = course.AttachmentFileType,
                    attachmentFileUrl = course.AttachmentFileUrl,
                    attachmentFileSize = course.AttachmentFileSize
                };

                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/courses", createDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding course: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var updateDto = new
                {
                    title = course.Title,
                    description = course.Description,
                    price = course.Price,
                    category = course.Category,
                    attachmentFileName = course.AttachmentFileName,
                    attachmentFileType = course.AttachmentFileType,
                    attachmentFileUrl = course.AttachmentFileUrl,
                    attachmentFileSize = course.AttachmentFileSize
                };

                var response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/courses/{course.Id}", updateDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating course: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/courses/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting course: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int courseId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}/favorites/{courseId}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding to favorites: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int courseId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/favorites/{courseId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing from favorites: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsFavoriteAsync(int userId, int courseId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/favorites/check/{courseId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<FavoriteCheckResponse>();
                    return result?.IsFavorite ?? false;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking favorite: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> PurchaseCourseAsync(int userId, int courseId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}/purchases/{courseId}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error purchasing course: {ex.Message}");
                return false;
            }
        }

        // Rating methods
        public async Task<Rating?> GetUserRatingAsync(int userId, int courseId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/ratings/course/{courseId}/user");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Rating>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user rating: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Rating>> GetCourseRatingsAsync(int courseId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/ratings/course/{courseId}");
                response.EnsureSuccessStatusCode();
                
                var ratings = await response.Content.ReadFromJsonAsync<List<Rating>>();
                return ratings ?? new List<Rating>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting course ratings: {ex.Message}");
                return new List<Rating>();
            }
        }

        public async Task<bool> SubmitRatingAsync(int userId, int courseId, int score, string? review)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var dto = new { score, review };
                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/ratings/course/{courseId}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting rating: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateRatingAsync(int ratingId, int score, string? review)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var dto = new { score, review };
                var response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/ratings/{ratingId}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating rating: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/ratings/{ratingId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting rating: {ex.Message}");
                return false;
            }
        }

        private class FavoriteCheckResponse
        {
            public bool IsFavorite { get; set; }
        }
    }
}
