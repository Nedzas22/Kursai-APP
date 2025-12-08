using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Collections.ObjectModel;
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
        private string _selectedCategory = string.Empty;

        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>
        {
            "Programming",
            "Web Development",
            "Mobile Development",
            "Data Science",
            "Machine Learning",
            "Artificial Intelligence",
            "Cybersecurity",
            "Cloud Computing",
            "DevOps",
            "Database",
            "Game Development",
            "UI/UX Design",
            "Graphic Design",
            "Digital Marketing",
            "Business",
            "Finance",
            "Accounting",
            "Project Management",
            "Leadership",
            "Photography",
            "Video Editing",
            "Music Production",
            "3D Modeling",
            "Animation",
            "Architecture",
            "Engineering",
            "Mathematics",
            "Science",
            "Language Learning",
            "Personal Development",
            "Health & Fitness",
            "Cooking",
            "Other"
        };

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

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddCourseViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Add Course";

            SaveCommand = new Command(async () => await SaveCourseAsync());
            CancelCommand = new Command(async () => await CancelAsync());
            
            // Set default category
            SelectedCategory = Categories[0];
        }

        private async Task SaveCourseAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(CourseTitle) || string.IsNullOrWhiteSpace(Description) || 
                string.IsNullOrWhiteSpace(Price) || string.IsNullOrWhiteSpace(SelectedCategory))
            {
                await Shell.Current.DisplayAlertAsync("Error", "All fields are required", "OK");
                return;
            }

            if (!decimal.TryParse(Price, out decimal priceValue) || priceValue < 0)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a valid price", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var user = await _authService.GetCurrentUserAsync();
                if (user == null)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "You must be logged in", "OK");
                    return;
                }

                var course = new Course
                {
                    Title = CourseTitle,
                    Description = Description,
                    Price = priceValue,
                    Category = SelectedCategory,
                    SellerId = user.Id,
                    SellerName = user.Username
                };

                var success = await _courseService.AddCourseAsync(course);

                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Course created successfully!", "OK");
                    // Navigate back - this will trigger OnAppearing and reload the courses
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to create course", "OK");
                }
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

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}