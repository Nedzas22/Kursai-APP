using Kursai.maui.ViewModels;

namespace Kursai.maui.Views
{
    public partial class MyCoursesPage : ContentPage
    {
        public MyCoursesPage(MyCoursesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}