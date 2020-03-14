using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class HorizontalShakeAnimation : BaseAnimation
    {
        public HorizontalShakeAnimation(int interval, double? parameter)
        {
            this.Interval = (uint)interval;
            this.Parameter = parameter.HasValue ? parameter.Value : 10;
        }

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            var self = (View)bindable;
            await self.TranslateTo(this.Parameter, 0, this.Interval);
            await self.TranslateTo(-this.Parameter, 0, this.Interval);
            return true;
        }

        protected override async Task StopAnimation(BindableObject bindable)
        {
            if(bindable is View self)
                await self.TranslateTo(0, 0, 100);
        }
    }
}
