using Kursai.maui.Models;
using Kursai.maui.Configuration;
using System.Net.Http.Json;

namespace Kursai.maui.Services
{
    public class ApiAuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private User? _currentUser;

        public ApiAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                var loginDto = new
                {
                    username = username,
                    password = password
                };

                System.Diagnostics.Debug.WriteLine($"Attempting login to: {ApiConfig.BaseUrl}/auth/login");
                
                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/auth/login", loginDto);
                
                System.Diagnostics.Debug.WriteLine($"Login response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    
                    if (result != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Login successful for user: {result.Username}");
                        
                        // Store token securely
                        await SecureStorage.SetAsync("auth_token", result.Token);
                        await SecureStorage.SetAsync("user_id", result.UserId.ToString());
                        await SecureStorage.SetAsync("username", result.Username);
                        await SecureStorage.SetAsync("email", result.Email);
                        
                        _currentUser = new User
                        {
                            Id = result.UserId,
                            Username = result.Username,
                            Email = result.Email
                        };
                        
                        return _currentUser;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
                }
                
                return null;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error during login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"API URL: {ApiConfig.BaseUrl}");
                throw new Exception($"Cannot connect to API server. Make sure the API is running at {ApiConfig.BaseUrl.Replace("/api", "")}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                throw new Exception($"Login error: {ex.Message}");
            }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            try
            {
                var registerDto = new
                {
                    username = username,
                    email = email,
                    password = password
                };

                System.Diagnostics.Debug.WriteLine($"Attempting registration to: {ApiConfig.BaseUrl}/auth/register");
                
                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/auth/register", registerDto);
                
                System.Diagnostics.Debug.WriteLine($"Registration response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    
                    if (result != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Registration successful for user: {result.Username}");
                        
                        // Store token securely
                        await SecureStorage.SetAsync("auth_token", result.Token);
                        await SecureStorage.SetAsync("user_id", result.UserId.ToString());
                        await SecureStorage.SetAsync("username", result.Username);
                        await SecureStorage.SetAsync("email", result.Email);
                        
                        _currentUser = new User
                        {
                            Id = result.UserId,
                            Username = result.Username,
                            Email = result.Email
                        };
                        
                        return true;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Registration failed: {response.StatusCode} - {errorContent}");
                }
                
                return false;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error during registration: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"API URL: {ApiConfig.BaseUrl}");
                throw new Exception($"Cannot connect to API server. Make sure the API is running at {ApiConfig.BaseUrl.Replace("/api", "")}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Registration error: {ex.Message}");
                throw new Exception($"Registration error: {ex.Message}");
            }
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            try
            {
                var userId = await SecureStorage.GetAsync("user_id");
                var username = await SecureStorage.GetAsync("username");
                var email = await SecureStorage.GetAsync("email");
                var token = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
                {
                    // Validate token with API
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/auth/validate");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                        
                        if (result != null)
                        {
                            _currentUser = new User
                            {
                                Id = result.UserId,
                                Username = result.Username,
                                Email = result.Email
                            };
                            
                            return _currentUser;
                        }
                    }
                    else
                    {
                        // Token is invalid, create user from stored data
                        _currentUser = new User
                        {
                            Id = int.Parse(userId),
                            Username = username,
                            Email = email ?? ""
                        };
                        return _currentUser;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get current user error: {ex.Message}");
            }

            return null;
        }

        public bool IsAuthenticated()
        {
            var token = SecureStorage.GetAsync("auth_token").Result;
            return !string.IsNullOrEmpty(token);
        }

        public async Task LogoutAsync()
        {
            _currentUser = null;
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("username");
            SecureStorage.Remove("email");
            await Task.CompletedTask;
        }

        private class AuthResponse
        {
            public string Token { get; set; } = string.Empty;
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }
    }
}
