using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Linq;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    public class CourseDetailsViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;
        private Course? _course;
        private bool _isFavorite;

        public Course? Course
        {
            get => _course;
            set => SetProperty(ref _course, value);
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        public ICommand ToggleFavoriteCommand { get; }
        public ICommand LoadCourseCommand { get; }
        public ICommand BackCommand { get; }

        public CourseDetailsViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Course Details";

            ToggleFavoriteCommand = new Command(async () => await ToggleFavoriteAsync());
            LoadCourseCommand = new Command<int>(async (courseId) => await LoadCourseAsync(courseId));
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

        private async Task LoadCourseAsync(int courseId)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                Course = await _courseService.GetCourseByIdAsync(courseId);

                var user = await _authService.GetCurrentUserAsync();
                if (user != null && Course != null)
                {
                    IsFavorite = await _courseService.IsFavoriteAsync(user.Id, courseId);
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

        private async Task ToggleFavoriteAsync()
        {
            if (Course == null) return;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please login to add favorites", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                bool success;
                if (IsFavorite)
                {
                    success = await _courseService.RemoveFromFavoritesAsync(user.Id, Course.Id);
                    if (success)
                    {
                        IsFavorite = false;
                        await Shell.Current.DisplayAlertAsync("Success", "Course removed from favorites", "OK");
                    }
                }
                else
                {
                    success = await _courseService.AddToFavoritesAsync(user.Id, Course.Id);
                    if (success)
                    {
                        IsFavorite = true;
                        await Shell.Current.DisplayAlertAsync("Success", "Course added to favorites!", "OK");
                    }
                }

                if (!success)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to update favorites", "OK");
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

