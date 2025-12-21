using Kursai.maui.Models;
using Kursai.maui.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kursai.maui.ViewModels
{
    [QueryProperty(nameof(CourseId), "courseId")]
    public class EditCourseViewModel : BaseViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;
        private int _courseId;
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

        public int CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                LoadCourseAsync(value);
            }
        }

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

        public EditCourseViewModel(ICourseService courseService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            Title = "Edit Course";

            SaveCommand = new Command(async () => await SaveCourseAsync());
            CancelCommand = new Command(async () => await CancelAsync());
            PickFileCommand = new Command(async () => await PickFileAsync());
            RemoveFileCommand = new Command(() => RemoveFile());
        }

        private async void LoadCourseAsync(int courseId)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var course = await _courseService.GetCourseByIdAsync(courseId);
                if (course != null)
                {
                    CourseTitle = course.Title;
                    Description = course.Description;
                    Price = course.Price.ToString("F2");
                    SelectedCategory = course.Category;
                    AttachmentFileName = course.AttachmentFileName;
                    AttachmentFileType = course.AttachmentFileType;
                    _attachmentFileUrl = course.AttachmentFileUrl;
                    _attachmentFileSize = course.AttachmentFileSize;
                    OnPropertyChanged(nameof(HasAttachment));
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
                    var extension = Path.GetExtension(result.FileName).ToLowerInvariant();
                    var validExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".pdf" };
                    
                    if (!validExtensions.Contains(extension))
                    {
                        await Shell.Current.DisplayAlert("Invalid File", 
                            "Please select a video file (.mp4, .avi, .mov, .wmv) or a PDF file.", "OK");
                        return;
                    }

                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    _attachmentFileData = memoryStream.ToArray();
                    
                    AttachmentFileName = result.FileName;
                    AttachmentFileType = result.ContentType ?? GetContentType(extension);
                    _attachmentFileSize = _attachmentFileData.Length;
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

            if (!HasAttachment)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please attach a video or PDF file", "OK");
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
                    Id = CourseId,
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

                var success = await _courseService.UpdateCourseAsync(course);

                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Course updated successfully!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to update course", "OK");
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
