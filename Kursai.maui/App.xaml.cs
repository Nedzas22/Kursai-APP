using Kursai.maui.Services;
using Kursai.maui.Views;
using Kursai.maui.ViewModels;

namespace Kursai.maui
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;

        public App(IAuthService authService)
        {
            InitializeComponent();

            _authService = authService;

            // Check authentication and set main page
            if (_authService.IsAuthenticated())
            {
                MainPage = new AppShell();
            }
            else
            {
                // Show login page outside of Shell
                MainPage = new AppShell(); // Let AppShell handle showing LoginPage
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage!);
        }

        // Call this after successful login to switch to Shell
        public static void SwitchToMainApp()
        {
            if (Application.Current is App app)
            {
                app.MainPage = new AppShell();
            }
        }
    }
}