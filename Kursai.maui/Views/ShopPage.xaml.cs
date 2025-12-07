using Kursai.maui.ViewModels;

namespace Kursai.maui.Views
{
    public partial class ShopPage : ContentPage
    {
        public ShopPage(ShopViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}

