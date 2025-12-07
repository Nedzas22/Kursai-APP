using Kursai.maui.Views;
using Kursai.maui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kursai.maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ShopPage), typeof(ShopPage));
            Routing.RegisterRoute(nameof(LibraryPage), typeof(LibraryPage));
            Routing.RegisterRoute(nameof(CourseDetailsPage), typeof(CourseDetailsPage));
            Routing.RegisterRoute(nameof(AddKursai), typeof(AddKursai));
            Routing.RegisterRoute(nameof(EditKursai), typeof(EditKursai));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(MyCoursesPage), typeof(MyCoursesPage));
            
            // Check authentication after Shell is loaded
            Loaded += AppShell_Loaded;
        }

        private async void AppShell_Loaded(object? sender, EventArgs e)
        {
            Loaded -= AppShell_Loaded;
            
            // Small delay to ensure Shell is fully initialized
            await Task.Delay(100);
            
            // Check if user is authenticated
            var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
            if (authService != null && authService.IsAuthenticated())
            {
                var user = await authService.GetCurrentUserAsync();
                if (user != null)
                {
                    // User is authenticated, navigate to MainTabs
                    await GoToAsync("//MainTabs");
                }
                else
                {
                    // User was marked as authenticated but user object is null - clear it
                    await authService.LogoutAsync();
                    await GoToAsync("//LoginPage");
                }
            }
            else
            {
                // Not authenticated, ensure we're on LoginPage
                await GoToAsync("//LoginPage");
            }
        }
    }
}
