using Kursai.maui.Models;

namespace Kursai.maui.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string username, string email, string password);
        Task<User> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<User> GetCurrentUserAsync();
        bool IsAuthenticated();
    }
}