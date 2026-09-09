using System;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
using Microsoft.Maui.ApplicationModel;
#endif

#if NET6_0_OR_GREATER
namespace Maui.Skeleton.Animations
#else
namespace Xamarin.Forms.Skeleton.Animations
#endif
{
    public abstract class BaseAnimation : IAnimation
    {
        public uint Interval { get; set; }
        public double Parameter { get; set; }

        protected abstract Task<bool> Animate(BindableObject bindable);

        protected abstract Task StopAnimation(BindableObject bindable);

        public void Start(BindableObject bindable) => RunOnMainThread(() => this.Run(bindable));

        public void Stop(BindableObject bindable) => RunOnMainThread(() => this.StopAnimation(bindable));

        /// <summary>
        /// Shortest run of Animate that can correspond to a real animation. Anything faster than
        /// a single 60fps frame means nothing was actually animated.
        /// </summary>
        private const int MinimumAnimatedMilliseconds = 16;

        private async Task Run(BindableObject bindable)
        {
            while (!Skeleton.GetCancelAnimation(bindable))
            {
                Skeleton.SetAnimating(bindable, true);

                var startedAt = DateTime.UtcNow;
                await Animate(bindable);

                // Platforms let the user turn animations off system wide (developer options,
                // battery savers, reduce-motion accessibility settings). Animate then returns
                // immediately and looping would snap the animated property between its two ends
                // as fast as the UI thread allows, which is what shows up as flickering. Settle
                // the view and leave the placeholder static instead, which is also what a user
                // who turned animations off is asking for.
                if ((DateTime.UtcNow - startedAt).TotalMilliseconds < MinimumAnimatedMilliseconds)
                {
                    await StopAnimation(bindable);
                    break;
                }
            }

            Skeleton.SetAnimating(bindable, false);
        }

        /// <summary>
        /// Animations have to be driven from the UI thread, which is where the animation manager
        /// of both frameworks expects to be called from. The loop used to be started on a
        /// threadpool thread instead and only kept working by accident.
        /// </summary>
        private static void RunOnMainThread(Func<Task> operation)
        {
#if NET6_0_OR_GREATER
            MainThread.BeginInvokeOnMainThread(() => _ = operation());
#else
            Device.BeginInvokeOnMainThread(() => _ = operation());
#endif
        }
    }
}
