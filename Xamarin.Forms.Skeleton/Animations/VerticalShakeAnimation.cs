using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class VerticalShakeAnimation : BaseAnimation
    {
        public VerticalShakeAnimation()
        {
            this.Interval = 150;
            this.Parameter = 15;
        }

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.TranslateTo(0, this.Parameter, this.Interval);
            await self.TranslateTo(0, -this.Parameter, this.Interval);
            return true;
        }

        protected override Task StopAnimation(BindableObject bindable)
        {
            throw new NotImplementedException();
        }
    }
}
