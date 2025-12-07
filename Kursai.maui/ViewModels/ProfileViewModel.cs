using Kursai.maui.Services;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly ICourseService _courseService;
        private string _username;
        private string _email;
        private string _userInitial;
        private int _coursesCreated;
        private int _favoritesCount;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string UserInitial
        {
            get => _userInitial;
            set => SetProperty(ref _userInitial, value);
        }

        public int CoursesCreated
        {
            get => _coursesCreated;
            set => SetProperty(ref _coursesCreated, value);
        }

        public int FavoritesCount
        {
            get => _favoritesCount;
            set => SetProperty(ref _favoritesCount, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand ViewMyCoursesCommand { get; }
        public ICommand ViewLibraryCommand { get; }
        public ICommand AddCourseCommand { get; }

        public ProfileViewModel(IAuthService authService, ICourseService courseService)
        {
            _authService = authService;
            _courseService = courseService;
            Title = "Profile";

            LogoutCommand = new Command(async () => await LogoutAsync());
            ViewMyCoursesCommand = new Command(async () => await ViewMyCoursesAsync());
            ViewLibraryCommand = new Command(async () => await ViewLibraryAsync());
            AddCourseCommand = new Command(async () => await AddCourseAsync());

            LoadProfileAsync();
        }

        private async Task LoadProfileAsync()
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user != null)
            {
                Username = user.Username;
                Email = user.Email;
                UserInitial = !string.IsNullOrEmpty(user.Username) 
                    ? user.Username.Substring(0, 1).ToUpper() 
                    : "U";

                var myCourses = await _courseService.GetMyCoursesAsync(user.Id);
                var favorites = await _courseService.GetFavoriteCoursesAsync(user.Id);

                CoursesCreated = myCourses.Count;
                FavoritesCount = favorites.Count;
            }
        }

        private async Task ViewMyCoursesAsync()
        {
            await Shell.Current.GoToAsync("MyCoursesPage");
        }

        private async Task ViewLibraryAsync()
        {
            await Shell.Current.GoToAsync("//MainTabs/LibraryPage");
        }

        private async Task AddCourseAsync()
        {
            await Shell.Current.GoToAsync("AddKursai");
        }

        private async Task LogoutAsync()
        {
            var confirm = await Shell.Current.DisplayAlertAsync("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (!confirm) return;

            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}