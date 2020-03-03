using System;
using System.Collections.Generic;
using SkeletonExample.ViewModels;
using Xamarin.Forms;

namespace SkeletonExample.Pages
{
    public partial class Page3 : BasePage
    {
        public Page3()
        {
            InitializeComponent();
            this.BindingContext = new Page3ViewModel();
        }
    }
}
