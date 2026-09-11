using SkeletonSample.ViewModels;

namespace SkeletonSample.Pages
{
    public partial class Aurora : ContentPage
    {
        public Aurora()
        {
            InitializeComponent();
            this.BindingContext = new AuroraViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);
        }
    }
}