using SkeletonSample.ViewModels;

namespace SkeletonSample.Pages
{
    public partial class Beat : ContentPage
    {
        public Beat()
        {
            InitializeComponent();
            this.BindingContext = new BeatViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);

            //if (Microsoft.Maui.Devices.DeviceInfo.Platform.Equals(Microsoft.Maui.Devices.DevicePlatform.iOS))
            //    mainGrid.Margin = new Thickness(30, On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets().Top + 30, 30, 30);
            //else
            //    mainGrid.Margin = new Thickness(30, 50, 30, 30);
        }
    }
}