using Kursai.maui.ViewModels;

namespace Kursai.maui.Views
{
    public partial class MyCoursesPage : ContentPage
    {
        private readonly MyCoursesViewModel _viewModel;

        public MyCoursesPage(MyCoursesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Reload courses when page appears to show newly created courses
            if (_viewModel.LoadCoursesCommand.CanExecute(null))
            {
                _viewModel.LoadCoursesCommand.Execute(null);
            }
        }
    }
}