using Kursai.maui.ViewModels;

namespace Kursai.maui.Views;

public partial class AddKursai : ContentPage
{
	public AddKursai(AddCourseViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    private void btnCanel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}