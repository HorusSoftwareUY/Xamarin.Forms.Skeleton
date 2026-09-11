namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Shared machinery for the animations that repaint the placeholder rather than changing a
    /// property of the view.
    /// </summary>
    /// <remarks>
    /// These paint the view's <see cref="VisualElement.Background"/> and change what is painted,
    /// which is what separates them from Fade, Beat and the shakes. Three consequences follow, and
    /// they are the same for every animation built on this:
    ///
    /// The animation has to be attached to the element that shows the placeholder colour. It is that
    /// element's own background, so it does not carry down to children the way Fade does, and on a
    /// transparent container it paints something nobody can see.
    ///
    /// It does not work on <c>Frame</c>. That control is deprecated in MAUI and its renderer does not
    /// repaint when the background is replaced, so the animation freezes on its first frame.
    /// <c>Border</c> repaints correctly.
    ///
    /// <see cref="BaseAnimation.Parameter"/> has no meaning. The look comes from
    /// <see cref="SweepColors"/>.
    /// </remarks>
    public abstract class BackgroundAnimation : BaseAnimation
    {
        /// <summary>
        /// Milliseconds between frames. 30fps rather than 60: these move slowly enough that 60 does
        /// not read as smoother, and a list of placeholders animates many views at once.
        /// </summary>
        protected const uint FrameRate = 33;

        /// <summary>
        /// Two or three colours. Two are read as edge and peak and mirrored, three are taken as
        /// written. Null falls back to <see cref="DefaultColorsFor"/>. Not every animation uses all
        /// three; see each one for what it takes.
        /// </summary>
        /// <remarks>
        /// Each colour's alpha is composited over the placeholder, so these describe light falling on
        /// it rather than replacing it. Written as <c>#AARRGGBB</c>, the order MAUI reads; CSS writes
        /// the same colour as <c>#RRGGBBAA</c>.
        /// </remarks>
        [System.ComponentModel.TypeConverter(typeof(SweepColorsTypeConverter))]
        public Color[]? SweepColors { get; set; }

        /// <summary>Colours to use when <see cref="SweepColors"/> was not set.</summary>
        protected abstract Color[] DefaultColorsFor(Color placeholder);

        /// <summary>
        /// Called once at the start of each pass, with the colour the animation plays over, so an
        /// animation can resolve anything that does not change while it runs.
        /// </summary>
        protected abstract void Prepare(Color placeholder);

        /// <summary>What to paint at this point of the pass, 0 at its start and 1 at its end.</summary>
        protected abstract Brush BrushAt(double progress);

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            if (bindable is not View view)
                return false;

            // Skeleton applies the placeholder colour before starting the animation, so this is the
            // colour the animation plays over. It only changes between passes.
            Prepare(view.BackgroundColor ?? Colors.Transparent);

            // Phase comes from a clock shared by every view, not from when this particular pass
            // started. Each view running its own stopwatch drifts, and a page full of placeholders
            // ends up out of step, where the reference has them move together. Reading the phase from
            // a process wide clock keeps them locked without any coordination between the animations.
            //
            // The pass still lasts a full interval of wall time. That keeps it honest for the guard
            // in BaseAnimation.Run, which stops an animation that returns faster than a frame, and it
            // means a busy UI thread drops frames rather than stretching the movement.
            var interval = Math.Max(1u, Interval);
            var began = Environment.TickCount64;

            while (!Skeleton.GetCancelAnimation(view))
            {
                var now = Environment.TickCount64;

                view.Background = BrushAt(now % interval / (double)interval);

                if (now - began >= interval)
                    break;

                await NextFrame(view);
            }

            return true;
        }

        protected override Task StopAnimation(BindableObject bindable)
        {
            if (bindable is View view)
            {
                // Clearing Background rather than assigning one puts the view back under whatever
                // BackgroundColor holds, which is what Skeleton restores separately.
                view.ClearValue(VisualElement.BackgroundProperty);
            }

            return Task.CompletedTask;
        }

        /// <summary>Waits one frame on the view's own dispatcher.</summary>
        static Task NextFrame(View view)
        {
            var next = new TaskCompletionSource<bool>();
            view.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(FrameRate), () => next.TrySetResult(true));
            return next.Task;
        }

        /// <summary>Expands the two colour form, edge and peak, into three.</summary>
        protected static Color[] Normalise(Color[] colors) => colors.Length switch
        {
            2 => [colors[0], colors[1], colors[0]],
            3 => colors,
            _ => throw new ArgumentException(
                $"This takes two or three colours, got {colors.Length}.", nameof(colors))
        };

        /// <summary>
        /// Composites a colour over the placeholder. What is painted replaces the view's background
        /// rather than sitting on top of it, so the translucency is resolved here.
        /// </summary>
        protected static Color Over(Color light, Color placeholder)
        {
            var alpha = light.Alpha;

            return new Color(
                light.Red * alpha + placeholder.Red * (1 - alpha),
                light.Green * alpha + placeholder.Green * (1 - alpha),
                light.Blue * alpha + placeholder.Blue * (1 - alpha),
                placeholder.Alpha);
        }

        /// <summary>Relative luminance, the sRGB coefficients used by WCAG.</summary>
        protected static double Luminance(Color color) =>
            0.2126 * color.Red + 0.7152 * color.Green + 0.0722 * color.Blue;
    }
}
