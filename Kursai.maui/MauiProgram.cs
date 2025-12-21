using Kursai.maui.Services;
using Kursai.maui.ViewModels;
using Kursai.maui.Views;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Maui.Platform;

namespace Kursai.maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    // Customize tab bar appearance on Android
                    handlers.AddHandler(typeof(Shell), typeof(Kursai.maui.Platforms.Android.CustomShellHandler));
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Configure HttpClient for API
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var httpClient = new HttpClient(new HttpClientHandler
                {
#if DEBUG
                    // Allow self-signed certificates in development
                    ServerCertificateCustomValidationCallback = 
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
                })
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
                
                return httpClient;
            });

            // Register API Services (Use these for real data persistence)
            builder.Services.AddSingleton<IAuthService, ApiAuthService>();
            builder.Services.AddSingleton<ICourseService, ApiCourseService>();

            // Register old in-memory services (commented out - use for offline mode)
            // builder.Services.AddSingleton<IAuthService, AuthService>();
            // builder.Services.AddSingleton<ICourseService, CourseService>();

            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<KursaiViewModel>();
            builder.Services.AddTransient<AddCourseViewModel>();
            builder.Services.AddTransient<EditCourseViewModel>();
            builder.Services.AddTransient<MyCoursesViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<LibraryViewModel>();
            builder.Services.AddTransient<ShopViewModel>();
            builder.Services.AddTransient<CourseDetailsViewModel>();

            // Register Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<KursaiPage>();
            builder.Services.AddTransient<AddKursai>();
            builder.Services.AddTransient<EditKursai>();
            builder.Services.AddTransient<MyCoursesPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<LibraryPage>();
            builder.Services.AddTransient<ShopPage>();
            builder.Services.AddTransient<CourseDetailsPage>();

            return builder.Build();
        }
    }
}
