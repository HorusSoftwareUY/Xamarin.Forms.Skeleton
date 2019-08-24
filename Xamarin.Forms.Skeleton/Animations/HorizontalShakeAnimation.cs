using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public class HorizontalShakeAnimation : BaseAnimation
    {
        public HorizontalShakeAnimation()
        {
            this.Interval = 100;
            this.Parameter = 10;
        }

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            Skeleton.SetAnimating(bindable, true);
            Layout self = (Layout)bindable;
            await self.TranslateTo(this.Parameter, 0, this.Interval);
            await self.TranslateTo(-this.Parameter, 0, this.Interval);
            return true;
        }

        protected override Task StopAnimation(BindableObject bindable)
        {
            throw new System.NotImplementedException();
        }
    }
}
