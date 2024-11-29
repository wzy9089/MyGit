namespace TouchBoard;
using CommunityToolkit.Maui.Views;

public partial class StrokeSettingsPopup : Popup
{
	public StrokeSettingsViewModel ViewModel { get; } = new StrokeSettingsViewModel();
    public StrokeSettingsPopup()
	{
        InitializeComponent();

		BindingContext = ViewModel;
	}
}