using Kursai.maui.Services;
using Kursai.maui.ViewModels;
using Kursai.maui.Views;
using Microsoft.Extensions.Logging;

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
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ICourseService, CourseService>();

            // Register ViewModels
            builder.Services.AddTransient<KursaiViewModel>();
            builder.Services.AddTransient<ShopViewModel>();
            builder.Services.AddTransient<CourseDetailsViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<MyCoursesViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<AddCourseViewModel>();
            builder.Services.AddTransient<LibraryViewModel>();

            // Register Views
            builder.Services.AddTransient<KursaiPage>();
            builder.Services.AddTransient<ShopPage>();
            builder.Services.AddTransient<LibraryPage>();
            builder.Services.AddTransient<CourseDetailsPage>();
            builder.Services.AddTransient<AddKursai>();
            builder.Services.AddTransient<EditKursai>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MyCoursesPage>();
            builder.Services.AddTransient<ProfilePage>();

            return builder.Build();
        }
    }
}
