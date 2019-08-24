using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class BeatAnimation : BaseAnimation
    {
        public BeatAnimation(int interval, double? parameter)
        {
            this.Interval = (uint)interval;
            this.Parameter = parameter.HasValue ? parameter.Value : 1.03;
        }

        public override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.ScaleTo(this.Parameter, this.Interval);
            await self.ScaleTo(1, this.Interval);
            return true;
        }
    }
}
