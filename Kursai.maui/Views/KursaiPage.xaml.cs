namespace Kursai.maui.Views;

using Kursai.maui.ViewModels;

public partial class KursaiPage : ContentPage
{
    public KursaiPage(KursaiViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}