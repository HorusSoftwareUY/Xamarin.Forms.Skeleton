using System;
using System.Collections.Generic;
using System.Text;
using Maui.Skeleton.Animations;

namespace SkeletonSample.ViewModels
{
    public class CustomAnimationViewModel : HorizontalShakeViewModel
    {
        public BaseAnimation MyCustomAnimation => new MyCustomAnimation();
    }
}