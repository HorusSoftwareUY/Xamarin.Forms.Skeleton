using SkeletonExample.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkeletonExample.Pages
{
    public partial class BasePage : ContentPage
    {
        public BasePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (BindingContext is BaseViewModel viewModel)
                viewModel.LoadCommand.Execute(null);
        }
    }
}