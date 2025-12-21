using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Kursai.maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Request storage permissions for Android 6.0+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                RequestStoragePermissions();
            }
        }

        private void RequestStoragePermissions()
        {
            var permissions = new List<string>();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Android 13+
            {
                permissions.Add(Android.Manifest.Permission.ReadMediaVideo);
                permissions.Add(Android.Manifest.Permission.ReadMediaImages);
            }
            else
            {
                permissions.Add(Android.Manifest.Permission.ReadExternalStorage);
                if (Build.VERSION.SdkInt <= BuildVersionCodes.S) // Android 12 and below
                {
                    permissions.Add(Android.Manifest.Permission.WriteExternalStorage);
                }
            }

            var permissionsToRequest = permissions
                .Where(p => CheckSelfPermission(p) != Permission.Granted)
                .ToArray();

            if (permissionsToRequest.Length > 0)
            {
                RequestPermissions(permissionsToRequest, 100);
            }
        }
    }
}
