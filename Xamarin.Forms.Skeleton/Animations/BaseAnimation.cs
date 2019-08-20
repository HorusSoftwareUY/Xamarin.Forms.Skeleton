using System;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public interface IAnimation
    {
        void Start(BindableObject bindable);
        Task<bool> Run(BindableObject bindable, double scaleTo, uint interval);
        void InitialStatus(BindableObject bindable);
        //Task<bool> Animate(BindableObject bindable, double scaleTo, uint interval);
    }

    public class BaseAnimation : IAnimation
    {
        public uint Interval { get; set; }
        public double Parameter { get; set; }

        public void InitialStatus(BindableObject bindable)
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

        public void Start(BindableObject bindable)
        {
            Task.Run(async () =>
            {
                var self = (Layout)bindable;
                double scaleTo = Skeleton.GetEscalation(bindable) > 0 ? Skeleton.GetEscalation(bindable) : 1.1;
                int animationTime = 800;//self.AnimationTime > 0 ? self.AnimationTime : self.DefaultAnimationTime;
                uint interval = (uint)animationTime / 2;
                await Run(bindable, scaleTo, interval);
            });
        }

        public async Task<bool> Run(BindableObject bindable, double scaleTo, uint interval)
        {
            if (Skeleton.GetCancelAnimation(bindable))
            {
                InitialStatus(bindable);
                return false;
            }
            await Animate(bindable);
            return await Run(bindable, scaleTo, interval);
        }

        public virtual async Task<bool> Animate(BindableObject bindable)
        {
            return true;
        }
    }
}
