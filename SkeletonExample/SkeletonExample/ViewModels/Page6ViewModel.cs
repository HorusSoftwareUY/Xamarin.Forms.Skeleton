using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Skeleton.Animations;

namespace SkeletonExample.ViewModels
{
    public class Page6ViewModel : Page5ViewModel
    {
        public BaseAnimation MyCustomAnimation => new MyCustomAnimation();
    }
}
