using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Skeleton.Animations;

namespace SkeletonExample
{
    public sealed class MyCustomAnimation : BaseAnimation
    {
        private const int DEFAULT_DURATION = 800;
        protected override async Task<bool> Animate(BindableObject bindable)
        {
            if (bindable is View target)
            {
                //Rotate to 90 degrees and fade to 75% of opacity
                await Task.WhenAll(target.FadeTo(0.75, DEFAULT_DURATION),
                    target.RotateYTo(5, DEFAULT_DURATION, Easing.Linear));

                //Rotate to -90 degrees and fade to 50% of opacity
                await Task.WhenAll(target.FadeTo(50),
                    target.RotateYTo(-5, DEFAULT_DURATION, Easing.Linear));

                //Reset rotation and fade to 100% of opacity
                await Task.WhenAll(target.FadeTo(1, DEFAULT_DURATION), target.RotateYTo(0, DEFAULT_DURATION));

                //Is animating
                return true;
            }

            return false;
        }

        protected override async Task StopAnimation(BindableObject bindable)
        {
            //Recovery state immediately
            if (bindable is View target)
                await Task.WhenAll(target.FadeTo(1, 1), target.RotateYTo(0, 1));
        }
    }
}
