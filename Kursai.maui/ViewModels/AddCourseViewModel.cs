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
        private string? _attachmentFileName;
        private string? _attachmentFileType;
        private string? _attachmentFileUrl;
        private long? _attachmentFileSize;
        private byte[]? _attachmentFileData;

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

        public string? AttachmentFileName
        {
            get => _attachmentFileName;
            set => SetProperty(ref _attachmentFileName, value);
        }

        public string? AttachmentFileType
        {
            get => _attachmentFileType;
            set => SetProperty(ref _attachmentFileType, value);
        }

        public bool HasAttachment => !string.IsNullOrEmpty(AttachmentFileName);

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PickFileCommand { get; }
        public ICommand RemoveFileCommand { get; }

        public AddCourseViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Add Course";

            SaveCommand = new Command(async () => await SaveCourseAsync());
            CancelCommand = new Command(async () => await CancelAsync());
            PickFileCommand = new Command(async () => await PickFileAsync());
            RemoveFileCommand = new Command(() => RemoveFile());
            
            // Set default category
            SelectedCategory = Categories[0];
        }

        private async Task PickFileAsync()
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.movie", "com.adobe.pdf" } },
                        { DevicePlatform.Android, new[] { "video/*", "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { ".mp4", ".avi", ".mov", ".wmv", ".pdf" } },
                        { DevicePlatform.macOS, new[] { "mp4", "avi", "mov", "wmv", "pdf" } }
                    });

                var options = new PickOptions
                {
                    PickerTitle = "Select a video or PDF file",
                    FileTypes = customFileType
                };

                var result = await FilePicker.Default.PickAsync(options);
                
                if (result != null)
                {
                    // Validate file type
                    var extension = Path.GetExtension(result.FileName).ToLowerInvariant();
                    var validExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".pdf" };
                    
                    if (!validExtensions.Contains(extension))
                    {
                        await Shell.Current.DisplayAlert("Invalid File", 
                            "Please select a video file (.mp4, .avi, .mov, .wmv) or a PDF file.", "OK");
                        return;
                    }

                    // Get file info
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    _attachmentFileData = memoryStream.ToArray();
                    
                    AttachmentFileName = result.FileName;
                    AttachmentFileType = result.ContentType ?? GetContentType(extension);
                    _attachmentFileSize = _attachmentFileData.Length;
                    
                    // Convert to base64 for storage/transmission
                    _attachmentFileUrl = Convert.ToBase64String(_attachmentFileData);
                    
                    OnPropertyChanged(nameof(HasAttachment));
                    
                    await Shell.Current.DisplayAlert("Success", 
                        $"File '{AttachmentFileName}' selected successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", 
                    $"Failed to pick file: {ex.Message}", "OK");
            }
        }

        private void RemoveFile()
        {
            AttachmentFileName = null;
            AttachmentFileType = null;
            _attachmentFileUrl = null;
            _attachmentFileSize = null;
            _attachmentFileData = null;
            OnPropertyChanged(nameof(HasAttachment));
        }

        private string GetContentType(string extension)
        {
            return extension switch
            {
                ".mp4" => "video/mp4",
                ".avi" => "video/x-msvideo",
                ".mov" => "video/quicktime",
                ".wmv" => "video/x-ms-wmv",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
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

            // Temporarily make file optional for testing
            // if (!HasAttachment)
            // {
            //     await Shell.Current.DisplayAlertAsync("Error", "Please attach a video or PDF file", "OK");
            //     return;
            // }

            if (!decimal.TryParse(Price, out decimal priceValue) || priceValue < 0)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a valid price", "OK");
                return;
            }

            // Check file size limit (50MB = 52428800 bytes)
            if (_attachmentFileSize.HasValue && _attachmentFileSize.Value > 52428800)
            {
                await Shell.Current.DisplayAlertAsync("Error", "File size must be less than 50MB", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var user = await _authService.GetCurrentUserAsync();
                if (user == null)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "You must be logged in", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var course = new Course
                {
                    Title = CourseTitle,
                    Description = Description,
                    Price = priceValue,
                    Category = SelectedCategory,
                    SellerId = user.Id,
                    SellerName = user.Username,
                    AttachmentFileName = AttachmentFileName,
                    AttachmentFileType = AttachmentFileType,
                    AttachmentFileUrl = _attachmentFileUrl,
                    AttachmentFileSize = _attachmentFileSize
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
                    await Shell.Current.DisplayAlertAsync("Error", 
                        "Failed to create course. Please ensure you're logged in and try again.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                if (httpEx.Message.Contains("401") || httpEx.Message.Contains("Unauthorized"))
                {
                    await Shell.Current.DisplayAlertAsync("Error", 
                        "Your session has expired. Please log in again.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", 
                        $"Network error: {httpEx.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    $"An error occurred: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error creating course: {ex}");
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