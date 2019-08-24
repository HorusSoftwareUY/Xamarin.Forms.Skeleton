using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class FadeAnimation : BaseAnimation
    {
        public FadeAnimation(int interval, double? parameter)
        {
            this.Interval = (uint)interval;
            this.Parameter = parameter.HasValue ? parameter.Value : 0.6;
        }

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.FadeTo(this.Parameter, this.Interval);
            await self.FadeTo(1, this.Interval);
            return true;
        }

        protected override async Task StopAnimation(BindableObject bindable)
        {
            Layout self = (Layout)bindable;
            await self.FadeTo(1, 100);
        }
    }
}
