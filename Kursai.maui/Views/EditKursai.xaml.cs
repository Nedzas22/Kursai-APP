namespace Kursai.maui.Views;

public partial class EditKursai : ContentPage
{
	public EditKursai()
	{
		InitializeComponent();
	}

    private void btnCanel_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync("..");
    }
}