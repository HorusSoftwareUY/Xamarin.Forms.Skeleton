using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public enum AnimationsPack
    {
        None,
        Beat,
        Fade,
        VerticalShake,
        HorizontalShake
    }

    public class BeatAnimation : BaseAnimation
    {
        public BeatAnimation()
        {
            Interval = 350;
            Parameter = 1.1;
        }

        public override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.ScaleTo(Parameter, Interval);
            await self.ScaleTo(1, Interval);
            return true;
        }
    }

    public class FadeAnimation : BaseAnimation
    {
        public FadeAnimation()
        {
            Interval = 700;
            Parameter = 90;
        }

        public override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.FadeTo(Parameter, Interval);
            await self.FadeTo(0, Interval);
            return true;
        }
    }

    public class VerticalShakeAnimation : BaseAnimation
    {
        public VerticalShakeAnimation()
        {
            Interval = 150;
            Parameter = 15;
        }

        public override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.TranslateTo(0, Parameter, Interval);
            await self.TranslateTo(0, -Parameter, Interval);
            return true;
        }
    }

    public class HorizontalShakeAnimation : BaseAnimation
    {
        public HorizontalShakeAnimation()
        {
            Interval = 100;
            Parameter = 10;
        }

        public override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.TranslateTo(Parameter, 0, Interval);
            await self.TranslateTo(-Parameter, 0, Interval);
            return true;
        }
    }
}
