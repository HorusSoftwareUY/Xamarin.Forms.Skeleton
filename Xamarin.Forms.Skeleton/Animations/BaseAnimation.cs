using System;
using System.Diagnostics;
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
        /// <summary>
        /// Shortest run of Animate that can correspond to a real animation. Anything faster than
        /// a single 60fps frame means nothing was actually animated.
        /// </summary>
        private const int MinimumAnimatedMilliseconds = 16;

        public uint Interval { get; set; }
        public double Parameter { get; set; }

        protected abstract Task<bool> Animate(BindableObject bindable);

        protected abstract Task StopAnimation(BindableObject bindable);

        public void Start(BindableObject bindable) => RunOnMainThread(() => this.Run(bindable));

        public void Stop(BindableObject bindable) => RunOnMainThread(() => this.StopAnimation(bindable));

        private async Task Run(BindableObject bindable)
        {
            try
            {
                while (!Skeleton.GetCancelAnimation(bindable))
                {
                    Skeleton.SetAnimating(bindable, true);

                    // Stopwatch rather than DateTime: the wall clock can jump backwards on an NTP
                    // sync and make a real animation look instantaneous, tripping the guard below.
                    var elapsed = Stopwatch.StartNew();
                    await Animate(bindable);

                    // Platforms let the user turn animations off system wide (developer options,
                    // battery savers). Animate then returns immediately and looping would snap the
                    // animated property between its two ends as fast as the UI thread allows, which
                    // is what shows up as flickering. Settle the view and leave the placeholder
                    // static instead, which is also what a user who turned animations off wants.
                    if (elapsed.ElapsedMilliseconds < MinimumAnimatedMilliseconds)
                    {
                        await StopAnimation(bindable);
                        break;
                    }
                }
            }
            finally
            {
                // Must run even if Animate throws: Skeleton.RunAnimation skips any view whose
                // Animating flag is set, so leaving it on would stop that view ever animating again.
                Skeleton.SetAnimating(bindable, false);
            }
        }

        /// <summary>
        /// Animations have to be driven from the UI thread, which is where the animation manager of
        /// both frameworks expects to be called from. The loop used to be started on a threadpool
        /// thread instead and only kept working by accident.
        /// </summary>
        private static void RunOnMainThread(Func<Task> operation)
        {
            Func<Task> observed = async () =>
            {
                try
                {
                    await operation();
                }
                catch (Exception exception)
                {
                    // A failing animation must never take the host application down, but the task
                    // still has to be awaited so the fault does not end up unobserved.
                    Debug.WriteLine($"Skeleton animation failed: {exception}");
                }
            };

#if NET6_0_OR_GREATER
            MainThread.BeginInvokeOnMainThread(() => _ = observed());
#else
            Device.BeginInvokeOnMainThread(() => _ = observed());
#endif
        }
    }
}
