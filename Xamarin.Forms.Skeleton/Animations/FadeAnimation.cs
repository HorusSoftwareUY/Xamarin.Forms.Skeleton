using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class FadeAnimation : BaseAnimation
    {
        public FadeAnimation()
        {
            this.Interval = 700;
            this.Parameter = 90;
        }

        public override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.FadeTo(this.Parameter, this.Interval);
            await self.FadeTo(0, this.Interval);
            return true;
        }
    }
}
