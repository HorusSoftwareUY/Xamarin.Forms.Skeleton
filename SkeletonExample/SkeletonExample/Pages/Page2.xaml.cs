using System;
using System.Collections.Generic;
using SkeletonExample.ViewModels;
using Xamarin.Forms;

namespace SkeletonExample.Pages
{
    public partial class Page2 : ContentPage
    {
        public Page2()
        {
            InitializeComponent();
            this.BindingContext = new Page2ViewModel();
        }

        protected override void OnAppearing()
        {
            var viewModel = this.BindingContext as Page2ViewModel;
            if (viewModel != null)
            {
                viewModel.LoadCommand.Execute(null);
            }
        }
    }
}
