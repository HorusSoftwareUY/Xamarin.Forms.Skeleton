using SkeletonSample.ViewModels;

namespace SkeletonSample.Pages
{
    public partial class Fade : ContentPage
    {
        public Fade()
        {
            InitializeComponent();
            this.BindingContext = new FadeViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);
        }
    }
}