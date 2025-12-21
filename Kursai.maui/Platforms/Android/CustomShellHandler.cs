using Android.Views;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace Kursai.maui.Platforms.Android
{
    public class CustomShellHandler : ShellRenderer
    {
        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new CustomBottomNavAppearanceTracker(this, shellItem);
        }
    }

    public class CustomBottomNavAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        public CustomBottomNavAppearanceTracker(IShellContext shellContext, ShellItem shellItem) 
            : base(shellContext, shellItem)
        {
        }

        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            base.SetAppearance(bottomView, appearance);

            // Remove the oval indicator by making it transparent
            var transparentColor = global::Android.Content.Res.ColorStateList.ValueOf(
                global::Android.Graphics.Color.Transparent);
            bottomView.ItemActiveIndicatorColor = transparentColor;
            
            // Set label visibility to always show
            bottomView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityLabeled;
            
            // Optional: Customize the item background to create an underline effect
            // You can set custom drawable or background here
        }
    }
}
