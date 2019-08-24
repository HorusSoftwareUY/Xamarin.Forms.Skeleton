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
                InitialStatus(bindable);
                return false;
            }
            else
            {
                await Animate(bindable);
                return await Run(bindable);
            }
        }

        private void InitialStatus(BindableObject bindable)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Layout self = bindable as Layout;
                Skeleton.SetAnimating(bindable, false);
                self.Children.ToList().ForEach(i => i.SetValue(VisualElement.IsVisibleProperty, true));

                self.Children.ToList().ForEach(i =>
                {
                    if (i is VisualElement)
                    {
                        VisualElement child = (VisualElement)i;
                        child.SetValue(VisualElement.IsVisibleProperty, true);
                        child.BackgroundColor = Skeleton.GetOriginalBackgroundColor(child);
                    }
                });
                self.BackgroundColor = Skeleton.GetOriginalBackgroundColor(bindable);
            });
        }
    }
}
