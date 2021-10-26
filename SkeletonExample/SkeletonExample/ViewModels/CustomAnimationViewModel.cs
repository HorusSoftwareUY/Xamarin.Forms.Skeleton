using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Skeleton.Animations;

namespace SkeletonExample.ViewModels
{
    public class CustomAnimationViewModel : HorizontalShakeViewModel
    {
        public BaseAnimation MyCustomAnimation => new MyCustomAnimation();
    }
}
