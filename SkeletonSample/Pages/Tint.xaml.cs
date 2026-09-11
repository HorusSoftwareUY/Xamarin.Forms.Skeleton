using SkeletonSample.ViewModels;

namespace SkeletonSample.Pages
{
    public partial class Tint : ContentPage
    {
        public Tint()
        {
            InitializeComponent();
            this.BindingContext = new TintViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);
        }
    }
}