namespace DLMS_Diplomka03.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) { Title = "DLMS_Diplomka03.Maui" };
	}
}
