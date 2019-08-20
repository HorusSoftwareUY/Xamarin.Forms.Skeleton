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
    }
}
