using SkeletonSample.ViewModels;

namespace SkeletonSample.Pages
{
    public partial class HorizontalShake : ContentPage
    {
        public HorizontalShake()
        {
            InitializeComponent();
            this.BindingContext = new HorizontalShakeViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);
        }
    }
}