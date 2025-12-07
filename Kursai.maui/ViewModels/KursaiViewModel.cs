using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    public class KursaiViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;
        private Course? _selectedCourse;

        public ObservableCollection<Course> Courses { get; } = new();

        public Course? SelectedCourse
        {
            get => _selectedCourse;
            set => SetProperty(ref _selectedCourse, value);
        }

        public ICommand LoadCoursesCommand { get; }
        public ICommand PurchaseCourseCommand { get; }
        public ICommand AddCourseCommand { get; }
        public ICommand CourseSelectedCommand { get; }

        public KursaiViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Available Courses";

            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            PurchaseCourseCommand = new Command<Course>(async (course) => await PurchaseCourseAsync(course));
            AddCourseCommand = new Command(async () => await NavigateToAddCourseAsync());
            CourseSelectedCommand = new Command<Course>(async (course) => await OnCourseSelectedAsync(course));

            _ = LoadCoursesAsync(); // Fire and forget with discard
        }

        private async Task LoadCoursesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                Courses.Clear();
                var courses = await _courseService.GetAllCoursesAsync();
                foreach (var course in courses)
                {
                    Courses.Add(course);
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

        private async Task PurchaseCourseAsync(Course course)
        {
            if (course == null) return;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var confirm = await Shell.Current.DisplayAlertAsync(
                "Get Course",
                $"Get '{course.Title}' for free?",
                "Yes", "No");

            if (!confirm) return;

            IsBusy = true;

            try
            {
                var success = await _courseService.PurchaseCourseAsync(user.Id, course.Id);

                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Course added to your library!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "You already have this course", "OK");
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

        private async Task NavigateToAddCourseAsync()
        {
            await Shell.Current.GoToAsync("AddKursai");
        }

        private async Task OnCourseSelectedAsync(Course course)
        {
            if (course == null) return;

            await Shell.Current.DisplayAlertAsync(
                course.Title,
                $"{course.Description}\n\nPrice: ${course.Price}\nSeller: {course.SellerName}\nCategory: {course.Category}",
                "OK");

            SelectedCourse = null;
        }
    }
}