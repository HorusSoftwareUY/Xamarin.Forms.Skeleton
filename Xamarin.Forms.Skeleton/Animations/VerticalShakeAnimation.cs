using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class VerticalShakeAnimation : BaseAnimation
    {
        public VerticalShakeAnimation(int interval, double? parameter)
        {
            this.Interval = (uint)interval;
            this.Parameter = parameter.HasValue ? parameter.Value : 15;
        }

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            var self = (View)bindable;
            await self.TranslateTo(0, this.Parameter, this.Interval);
            await self.TranslateTo(0, -this.Parameter, this.Interval);
            return true;
        }

        protected override async Task StopAnimation(BindableObject bindable)
        {
            if (bindable is View self)
                await self.TranslateTo(0, 0, 100);
        }
    }
}
