namespace Kursai.maui.Configuration
{
    public static class ApiConfig
    {
        // API URL Configuration
        // Update this with your actual API URL based on your platform
        
#if ANDROID
        // For Android Emulator: use 10.0.2.2 instead of localhost
        public const string BaseUrl = "http://10.0.2.2:7128/api";
#else
        // For Windows, iOS Simulator, and Mac
        public const string BaseUrl = "https://localhost:7128/api";
#endif
        
        // For production deployment, replace with your deployed API URL:
        // public const string BaseUrl = "https://yourapi.azurewebsites.net/api";
        
        // Alternative HTTP endpoint (if HTTPS causes issues):
        // public const string BaseUrl = "http://localhost:5265/api";
    }
}
