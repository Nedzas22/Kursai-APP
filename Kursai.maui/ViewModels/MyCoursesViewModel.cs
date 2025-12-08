using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    public class MyCoursesViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;

        public ObservableCollection<Course> Courses { get; } = new();

        public ICommand LoadCoursesCommand { get; }
        public ICommand AddCourseCommand { get; }
        public ICommand EditCourseCommand { get; }
        public ICommand DeleteCourseCommand { get; }
        public ICommand BackCommand { get; }

        public MyCoursesViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "My Created Courses";

            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            AddCourseCommand = new Command(async () => await AddCourseAsync());
            EditCourseCommand = new Command<Course>(async (course) => await EditCourseAsync(course));
            DeleteCourseCommand = new Command<Course>(async (course) => await DeleteCourseAsync(course));
            BackCommand = new Command(async () => await GoBackAsync());
        }

        private async Task GoBackAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                // Fallback: try alternative navigation
                if (Shell.Current.Navigation.NavigationStack.Count > 1)
                {
                    await Shell.Current.Navigation.PopAsync();
                }
            }
        }

        private async Task LoadCoursesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var user = await _authService.GetCurrentUserAsync();
                if (user == null) return;

                Courses.Clear();
                var courses = await _courseService.GetMyCoursesAsync(user.Id);
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

        private async Task AddCourseAsync()
        {
            await Shell.Current.GoToAsync("AddKursai");
        }

        private async Task EditCourseAsync(Course course)
        {
            if (course == null) return;
            await Shell.Current.GoToAsync($"EditKursai?courseId={course.Id}");
        }

        private async Task DeleteCourseAsync(Course course)
        {
            if (course == null) return;

            var confirm = await Shell.Current.DisplayAlertAsync(
                "Delete Course",
                $"Are you sure you want to delete '{course.Title}'?",
                "Yes", "No");

            if (!confirm) return;

            IsBusy = true;

            try
            {
                var success = await _courseService.DeleteCourseAsync(course.Id);
                if (success)
                {
                    Courses.Remove(course);
                    await Shell.Current.DisplayAlertAsync("Success", "Course deleted successfully", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to delete course", "OK");
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
    }
}