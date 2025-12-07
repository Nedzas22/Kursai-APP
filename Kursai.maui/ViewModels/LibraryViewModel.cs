using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    public class LibraryViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;

        public ObservableCollection<Course> Courses { get; } = new();

        public ICommand LoadCoursesCommand { get; }
        public ICommand CourseTappedCommand { get; }

        public LibraryViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Library";

            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            CourseTappedCommand = new Command<Course>(async (course) => await OnCourseTappedAsync(course));

            LoadCoursesAsync();
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
                var courses = await _courseService.GetFavoriteCoursesAsync(user.Id);
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

        private async Task OnCourseTappedAsync(Course course)
        {
            if (course == null) return;
            await Shell.Current.GoToAsync($"CourseDetailsPage?courseId={course.Id}");
        }
    }
}

