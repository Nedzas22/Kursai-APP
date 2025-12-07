using Kursai.maui.Models;
using System.Security.Cryptography;
using System.Text;

namespace Kursai.maui.Services
{
    public class AuthService : IAuthService
    {
        private User _currentUser;
        private readonly List<User> _users = new();

        public AuthService()
        {
            // Demo data
            _users.Add(new User
            {
                Id = 1,
                Username = "demo",
                Email = "demo@example.com",
                PasswordHash = HashPassword("demo123"),
                CreatedAt = DateTime.Now
            });
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            await Task.Delay(500); // Simulate API call

            if (_users.Any(u => u.Username == username || u.Email == email))
                return false;

            var user = new User
            {
                Id = _users.Count + 1,
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.Now
            };

            _users.Add(user);
            _currentUser = user;
            await SaveUserToPreferences(user);
            return true;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            await Task.Delay(500); // Simulate API call

            var passwordHash = HashPassword(password);
            var user = _users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);

            if (user != null)
            {
                _currentUser = user;
                await SaveUserToPreferences(user);
            }

            return user;
        }

        public async Task LogoutAsync()
        {
            _currentUser = null;
            Preferences.Remove("UserId");
            Preferences.Remove("Username");
            Preferences.Remove("UserEmail");
            Preferences.Remove("UserCreatedAt");
            await Task.CompletedTask;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            var userId = Preferences.Get("UserId", 0);
            if (userId > 0)
            {
                // First try to find in memory
                _currentUser = _users.FirstOrDefault(u => u.Id == userId);
                
                // If not found, try to reconstruct from preferences
                if (_currentUser == null)
                {
                    var username = Preferences.Get("Username", string.Empty);
                    var email = Preferences.Get("UserEmail", string.Empty);
                    if (!string.IsNullOrEmpty(username))
                    {
                        _currentUser = new User
                        {
                            Id = userId,
                            Username = username,
                            Email = email,
                            CreatedAt = Preferences.Get("UserCreatedAt", DateTime.Now)
                        };
                        // Add to users list so it persists
                        _users.Add(_currentUser);
                    }
                }
            }

            return await Task.FromResult(_currentUser);
        }

        public bool IsAuthenticated()
        {
            return _currentUser != null || Preferences.Get("UserId", 0) > 0;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private async Task SaveUserToPreferences(User user)
        {
            Preferences.Set("UserId", user.Id);
            Preferences.Set("Username", user.Username);
            Preferences.Set("UserEmail", user.Email);
            Preferences.Set("UserCreatedAt", user.CreatedAt);
            await Task.CompletedTask;
        }
    }
}