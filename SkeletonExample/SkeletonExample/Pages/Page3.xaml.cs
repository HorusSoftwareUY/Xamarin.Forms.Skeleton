using System;
using System.Collections.Generic;
using SkeletonExample.ViewModels;
using Xamarin.Forms;

namespace SkeletonExample.Pages
{
    public partial class Page3 : ContentPage
    {
        public Page3()
        {
            InitializeComponent();
            this.BindingContext = new Page3ViewModel();
        }

        protected override void OnAppearing()
        {
            var viewModel = this.BindingContext as Page3ViewModel;
            if (viewModel != null)
            {
                viewModel.LoadCommand.Execute(null);
            }
        }
    }
}
