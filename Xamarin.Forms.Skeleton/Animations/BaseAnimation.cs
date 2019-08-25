using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public abstract class BaseAnimation : IAnimation
    {
        public uint Interval { get; set; }
        public double Parameter { get; set; }
        
        protected abstract Task<bool> Animate(BindableObject bindable);

        protected abstract Task StopAnimation(BindableObject bindable);

        public void Start(BindableObject bindable)
        {
            Task.Run(async () => { await this.Run(bindable); });
        }

        public void Stop(BindableObject bindable)
        {
            Task.Run(async () => { await this.StopAnimation(bindable); });
        }

        private async Task<bool> Run(BindableObject bindable)
        {
            if (Skeleton.GetCancelAnimation(bindable))
            {
                Skeleton.SetAnimating(bindable, false);
                return false;
            }
            else
            {
                await Animate(bindable);
                return await Run(bindable);
            }
        }
    }
}
