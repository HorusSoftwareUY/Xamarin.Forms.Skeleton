using SkeletonSample.ViewModels;

namespace SkeletonSample.Pages
{
    public partial class Shimmer : ContentPage
    {
        public Shimmer()
        {
            InitializeComponent();
            this.BindingContext = new ShimmerViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);
        }
    }
}