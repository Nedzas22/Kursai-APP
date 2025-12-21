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
        private bool _isPurchased;
        private bool _isOwnCourse;
        private Rating? _userRating;
        private int _selectedRating;
        private string _reviewText = string.Empty;

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

        public bool IsPurchased
        {
            get => _isPurchased;
            set => SetProperty(ref _isPurchased, value);
        }

        public bool IsOwnCourse
        {
            get => _isOwnCourse;
            set => SetProperty(ref _isOwnCourse, value);
        }

        public Rating? UserRating
        {
            get => _userRating;
            set
            {
                SetProperty(ref _userRating, value);
                OnPropertyChanged(nameof(HasUserRated));
                OnPropertyChanged(nameof(CanRate));
            }
        }

        public int SelectedRating
        {
            get => _selectedRating;
            set => SetProperty(ref _selectedRating, value);
        }

        public string ReviewText
        {
            get => _reviewText;
            set => SetProperty(ref _reviewText, value);
        }

        public bool CanAccessFile => IsOwnCourse || IsPurchased;
        public bool HasUserRated => UserRating != null;
        public bool CanRate => (IsOwnCourse || IsPurchased) && !HasUserRated;

        public ICommand ToggleFavoriteCommand { get; }
        public ICommand LoadCourseCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ViewFileCommand { get; }
        public ICommand PurchaseCourseCommand { get; }
        public ICommand SubmitRatingCommand { get; }

        public CourseDetailsViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Course Details";

            ToggleFavoriteCommand = new Command(async () => await ToggleFavoriteAsync());
            LoadCourseCommand = new Command<int>(async (courseId) => await LoadCourseAsync(courseId));
            BackCommand = new Command(async () => await GoBackAsync());
            ViewFileCommand = new Command(async () => await ViewFileAsync());
            PurchaseCourseCommand = new Command(async () => await PurchaseCourseAsync());
            SubmitRatingCommand = new Command(async () => await SubmitRatingAsync());
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
                    
                    // Check if user owns this course
                    IsOwnCourse = Course.SellerId == user.Id;
                    
                    // Check if user has purchased this course
                    var purchasedCourses = await _courseService.GetPurchasedCoursesAsync(user.Id);
                    IsPurchased = purchasedCourses.Any(c => c.Id == courseId);
                    
                    // Load user rating if available
                    UserRating = await _courseService.GetUserRatingAsync(user.Id, courseId);
                    
                    OnPropertyChanged(nameof(CanAccessFile));
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

        private async Task ViewFileAsync()
        {
            if (Course == null || string.IsNullOrEmpty(Course.AttachmentFileName))
            {
                await Shell.Current.DisplayAlertAsync("Error", "No file attached to this course", "OK");
                return;
            }

            if (!CanAccessFile)
            {
                await Shell.Current.DisplayAlertAsync("Access Denied", 
                    "Please purchase this course to access the materials", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Course.AttachmentFileUrl))
            {
                await Shell.Current.DisplayAlertAsync("Error", "File data is not available", "OK");
                return;
            }

            // Ask user what they want to do
            var action = await Shell.Current.DisplayActionSheet(
                "Course Material",
                "Cancel",
                null,
                "Open File",
                "Save to Downloads");

            if (action == "Cancel" || action == null)
                return;

            IsBusy = true;

            try
            {
                // Decode base64 string to bytes
                byte[] fileBytes = Convert.FromBase64String(Course.AttachmentFileUrl);

                if (action == "Open File")
                {
                    await OpenFileAsync(fileBytes);
                }
                else if (action == "Save to Downloads")
                {
                    await SaveFileAsync(fileBytes);
                }
            }
            catch (FormatException)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    "File data is corrupted or invalid", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    $"Failed to process file: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenFileAsync(byte[] fileBytes)
        {
            try
            {
                // Create a temporary file path
                string fileName = $"course_{Course!.Id}_{Course.AttachmentFileName}";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Write the file to disk
                await File.WriteAllBytesAsync(filePath, fileBytes);

                // Open the file with the default application
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });

                await Shell.Current.DisplayAlertAsync("Success", 
                    "File opened successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    $"Failed to open file: {ex.Message}", "OK");
            }
        }

        private async Task SaveFileAsync(byte[] fileBytes)
        {
            try
            {
                // Get the Downloads folder path (platform-specific)
                string downloadsPath;

#if ANDROID
                downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryDownloads)?.AbsolutePath 
                    ?? Path.Combine(FileSystem.AppDataDirectory, "Downloads");
#elif WINDOWS
                downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                    "Downloads");
#elif IOS || MACCATALYST
                downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                    "Downloads");
#else
                downloadsPath = FileSystem.AppDataDirectory;
#endif

                // Ensure directory exists
                Directory.CreateDirectory(downloadsPath);

                // Create the file path
                string fileName = Course!.AttachmentFileName!;
                string filePath = Path.Combine(downloadsPath, fileName);

                // If file exists, add a number
                int counter = 1;
                while (File.Exists(filePath))
                {
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(Course.AttachmentFileName);
                    string extension = Path.GetExtension(Course.AttachmentFileName);
                    fileName = $"{nameWithoutExt}({counter}){extension}";
                    filePath = Path.Combine(downloadsPath, fileName);
                    counter++;
                }

                // Write the file
                await File.WriteAllBytesAsync(filePath, fileBytes);

                await Shell.Current.DisplayAlertAsync("Success", 
                    $"File saved to Downloads folder:\n{fileName}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    $"Failed to save file: {ex.Message}", "OK");
            }
        }

        private string FormatFileSize(long? fileSize)
        {
            if (!fileSize.HasValue) return "Unknown size";

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = fileSize.Value;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
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

        private async Task PurchaseCourseAsync()
        {
            if (Course == null) return;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please login to purchase courses", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // Check if the course is already purchased
                var purchasedCourses = await _courseService.GetPurchasedCoursesAsync(user.Id);
                if (purchasedCourses.Any(c => c.Id == Course.Id))
                {
                    await Shell.Current.DisplayAlertAsync("Info", "You have already purchased this course", "OK");
                    return;
                }

                // Proceed with the purchase
                bool success = await _courseService.PurchaseCourseAsync(user.Id, Course.Id);
                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Course purchased successfully! You can now access the materials.", "OK");

                    // Update the local course state
                    IsPurchased = true;
                    OnPropertyChanged(nameof(CanAccessFile));
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to purchase the course. Please try again.", "OK");
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

        private async Task SubmitRatingAsync()
        {
            if (Course == null) return;

            if (SelectedRating == 0)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please select a rating", "OK");
                return;
            }

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please login to submit your rating", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                // Submit the rating and review
                bool success = await _courseService.SubmitRatingAsync(user.Id, Course.Id, SelectedRating, ReviewText);
                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Thank you for your feedback!", "OK");

                    // Reload course to get updated ratings
                    await LoadCourseAsync(Course.Id);

                    // Clear the rating and review fields
                    SelectedRating = 0;
                    ReviewText = string.Empty;
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to submit your rating. You may have already rated this course.", "OK");
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

