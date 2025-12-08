using Kursai.maui.ViewModels;

namespace Kursai.maui.Views
{
    public partial class ShopPage : ContentPage
    {
        private readonly ShopViewModel _viewModel;

        public ShopPage(ShopViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Reload courses when page appears
            if (_viewModel.LoadCoursesCommand.CanExecute(null))
            {
                _viewModel.LoadCoursesCommand.Execute(null);
            }
        }
    }
}

