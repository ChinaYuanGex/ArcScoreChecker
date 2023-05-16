using CommunityToolkit.Maui.Alerts;
using Plugin.LocalNotification;

namespace ArcaeaScoreChecker;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	public static MainPage mpEntry;
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
		string basedir = FileSystem.AppDataDirectory;
		FileProcessor.TryCreateDefaultFileIfFirst();
		GlobalData.initAllData();
        ScoreData.Best.OpenDatabase();
        ScoreData.Recent.OpenDatabase();
		mpEntry = this;

    }
}
