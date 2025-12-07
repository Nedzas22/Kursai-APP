using Kursai.maui.ViewModels;

namespace Kursai.maui.Views
{
    public partial class LibraryPage : ContentPage
    {
        public LibraryPage(LibraryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is LibraryViewModel vm)
            {
                vm.LoadCoursesCommand.Execute(null);
            }
        }
    }
}

