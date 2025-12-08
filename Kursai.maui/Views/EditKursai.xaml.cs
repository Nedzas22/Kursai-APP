using Kursai.maui.ViewModels;

namespace Kursai.maui.Views;

public partial class EditKursai : ContentPage
{
	public EditKursai(EditCourseViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}