using SkeletonSample.ViewModels;

namespace SkeletonSample.Pages
{
    public partial class Skeleton : ContentPage
    {
        public Skeleton()
        {
            InitializeComponent();
            this.BindingContext = new SkeletonViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);
        }
    }
}