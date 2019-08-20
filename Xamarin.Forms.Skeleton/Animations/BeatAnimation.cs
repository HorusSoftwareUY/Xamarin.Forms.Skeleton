using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class BeatAnimation : BaseAnimation
    {
        public BeatAnimation()
        {
            this.Interval = 350;
            this.Parameter = 1.1;
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
