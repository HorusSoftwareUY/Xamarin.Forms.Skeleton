using SkeletonExample.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SkeletonExample.Pages
{
    public partial class CustomAnimation : BasePage
    {
        public CustomAnimation()
        {
            InitializeComponent();
            BindingContext = new CustomAnimationViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Device.RuntimePlatform.Equals(Device.iOS))
                mainGrid.Margin = new Thickness(30, On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets().Top + 30, 30, 30);
            else
                mainGrid.Margin = new Thickness(30, 50, 30, 30);
        }
    }
}