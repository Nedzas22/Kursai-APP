using Kursai.maui.ViewModels;

namespace Kursai.maui.Views
{
    [QueryProperty(nameof(CourseId), "courseId")]
    public partial class CourseDetailsPage : ContentPage
    {
        public string CourseId { get; set; }

        public CourseDetailsPage(CourseDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is CourseDetailsViewModel vm && !string.IsNullOrEmpty(CourseId))
            {
                if (int.TryParse(CourseId, out int courseId))
                {
                    vm.LoadCourseCommand.Execute(courseId);
                }
            }
        }
    }
}

