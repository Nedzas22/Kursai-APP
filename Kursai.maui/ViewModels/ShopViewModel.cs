using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    public class ShopViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;
        private string _searchText = string.Empty;
        private List<Course> _allCourses = new();

        public ObservableCollection<Course> Courses { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterCourses();
                }
            }
        }

        public ICommand LoadCoursesCommand { get; }
        public ICommand CourseTappedCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }

        public ShopViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Shop";

            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            CourseTappedCommand = new Command<Course>(async (course) => await OnCourseTappedAsync(course));
            ToggleFavoriteCommand = new Command<Course>(async (course) => await ToggleFavoriteAsync(course));

            LoadCoursesAsync();
        }

        private async Task LoadCoursesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                _allCourses = await _courseService.GetAllCoursesAsync();
                FilterCourses();
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

        private void FilterCourses()
        {
            Courses.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var course in _allCourses)
                {
                    Courses.Add(course);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                var filtered = _allCourses.Where(c =>
                    c.Title.ToLower().Contains(searchLower) ||
                    c.Description.ToLower().Contains(searchLower) ||
                    c.Category.ToLower().Contains(searchLower) ||
                    c.SellerName.ToLower().Contains(searchLower)
                ).ToList();

                foreach (var course in filtered)
                {
                    Courses.Add(course);
                }
            }
        }

        private async Task OnCourseTappedAsync(Course course)
        {
            if (course == null) return;
            await Shell.Current.GoToAsync($"CourseDetailsPage?courseId={course.Id}");
        }

        private async Task ToggleFavoriteAsync(Course course)
        {
            if (course == null) return;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please login to add favorites", "OK");
                return;
            }

            try
            {
                var isFavorite = await _courseService.IsFavoriteAsync(user.Id, course.Id);
                
                if (isFavorite)
                {
                    var success = await _courseService.RemoveFromFavoritesAsync(user.Id, course.Id);
                    if (success)
                    {
                        await Shell.Current.DisplayAlertAsync("Success", "Removed from favorites", "OK");
                    }
                }
                else
                {
                    var success = await _courseService.AddToFavoritesAsync(user.Id, course.Id);
                    if (success)
                    {
                        await Shell.Current.DisplayAlertAsync("Success", "Added to favorites!", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

