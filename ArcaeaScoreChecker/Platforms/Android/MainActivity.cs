using Android.App;
using Android.Content.PM;
using Android.OS;
using ArcaeaScoreChecker.Platforms.Android;
using System.ComponentModel;
using Plugin.LocalNotification;

namespace ArcaeaScoreChecker;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        GlobalLayoutUtil.AssistActivity(this);
    }
}
