using SkeletonExample.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SkeletonExample.Pages
{
    public partial class Page2 : BasePage
    {
        public Page2()
        {
            InitializeComponent();
            this.BindingContext = new Page2ViewModel();
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
