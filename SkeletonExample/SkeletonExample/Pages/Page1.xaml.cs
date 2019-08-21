using System;
using System.Collections.Generic;
using SkeletonExample.ViewModels;
using Xamarin.Forms;

namespace SkeletonExample.Pages
{
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
            this.BindingContext = new Page1ViewModel();
        }

        protected override void OnAppearing()
        {
            var viewModel = this.BindingContext as Page1ViewModel;
            if (viewModel != null)
            {
                viewModel.LoadCommand.Execute(null);
            }
        }
    }
}
