using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    public class AddCourseViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;
        private string _courseTitle = string.Empty;
        private string _description = string.Empty;
        private string _price = string.Empty;
        private string _category = string.Empty;

        public string CourseTitle
        {
            get => _courseTitle;
            set => SetProperty(ref _courseTitle, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddCourseViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Add Course";

            SaveCommand = new Command(async () => await SaveCourseAsync());
            CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private async Task SaveCourseAsync()
        {
            if (string.IsNullOrWhiteSpace(CourseTitle) || string.IsNullOrWhiteSpace(Price))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Title and Price are required", "OK");
                return;
            }

            if (!decimal.TryParse(Price, out var price))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Invalid price format", "OK");
                return;
            }

            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var course = new Course
            {
                Title = CourseTitle,
                Description = Description ?? string.Empty,
                Price = price,
                Category = Category ?? "General",
                SellerId = user.Id,
                SellerName = user.Username,
                ImageUrl = "dotnet_bot.png"
            };

            IsBusy = true;

            try
            {
                await _courseService.AddCourseAsync(course);
                await Shell.Current.DisplayAlertAsync("Success", "Course added successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}