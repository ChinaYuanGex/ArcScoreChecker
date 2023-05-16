
namespace ArcaeaScoreChecker;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        Entry = this;

        MainPage = new MainPage();
	}
	public static App Entry;
}
