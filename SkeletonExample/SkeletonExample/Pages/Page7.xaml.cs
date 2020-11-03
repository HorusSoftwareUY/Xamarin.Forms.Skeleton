using System;
using System.Collections.Generic;
using SkeletonExample.ViewModels;
using Xamarin.Forms;

namespace SkeletonExample.Pages
{
    public partial class Page7 : BasePage
    {
        public Page7()
        {
            InitializeComponent();
            BindingContext = new Page7ViewModel();
        }
    }
}
