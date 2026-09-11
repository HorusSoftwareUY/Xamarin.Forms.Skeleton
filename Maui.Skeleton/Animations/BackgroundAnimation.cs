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
        public Color[]? SweepColors
        {
            get => _sweepColors;

            // Checked on the way in, not where the colours are used. Past this point the animation
            // runs on a fire and forget task whose exceptions BaseAnimation only writes to debug
            // output, so a wrong length assigned from code would surface as a placeholder that
            // silently never moves. The XAML converter reports the same mistake at parse time.
            set
            {
                if (value is not null && value.Length is not (2 or 3))
                    throw new ArgumentException(
                        $"Sweep colours take two or three values, got {value.Length}. Two are read as edge and peak and mirrored, three as written.",
                        nameof(SweepColors));

                _sweepColors = value;
            }
        }

        Color[]? _sweepColors;

        /// <summary>Colours to use when <see cref="SweepColors"/> was not set.</summary>
        protected abstract Color[] DefaultColorsFor(Color placeholder);

        /// <summary>
        /// Resolves the colours the pass will paint with, given the placeholder it plays over.
        /// </summary>
        /// <remarks>
        /// The result is handed back to <see cref="BrushAt"/> rather than kept on the animation.
        /// One instance can drive several views at once, which is why every internal flag Skeleton
        /// keeps lives on the view, and pass state has to do the same: held here, two views with
        /// different placeholder colours would both paint whichever prepared last.
        /// </remarks>
        protected abstract Color[] Prepare(Color placeholder);

        /// <summary>What to paint at this point of the pass, 0 at its start and 1 at its end.</summary>
        protected abstract Brush BrushAt(Color[] colors, double progress);

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            if (bindable is not View view)
                return false;

            // Platforms let the user turn animations off system wide, and BaseAnimation.Run stops any
            // animation whose pass returns faster than a frame for exactly that reason. This loop
            // paces itself, so it would sail past that guard and keep moving after the user asked for
            // stillness. Probing a real platform animation first is what makes the guard apply here
            // too: with animations off it returns at once, so this pass does as well, and Run settles
            // the view into a static placeholder.
            if (!await PlatformAnimates(view))
                return false;

            // Skeleton applies the placeholder colour before starting the animation, so this is the
            // colour the animation plays over. It only changes between passes.
            var colors = Prepare(view.BackgroundColor ?? Colors.Transparent);

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

                view.Background = BrushAt(colors, now % interval / (double)interval);

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

        /// <summary>
        /// Whether the platform is actually animating, asked once and remembered.
        /// </summary>
        /// <remarks>
        /// Fades the view to the opacity it already has, so nothing shows, and times it: a real
        /// animation takes about as long as it was asked for, while one the system has disabled
        /// returns immediately.
        ///
        /// Cached because it is not free. A page of placeholders starts two dozen of these at once
        /// and the probe queues behind all of them; measured on a moto g54, one that asked for 32 ms
        /// came back after 1039. Paid once per pass that would have eaten a large part of every one.
        /// The setting it reads does not change while an app is in front of someone.
        /// </remarks>
        static Task<bool>? _platformAnimates;

        static Task<bool> PlatformAnimates(View view) => _platformAnimates ??= Probe(view);

        static async Task<bool> Probe(View view)
        {
            // Long enough that scheduling noise cannot be mistaken for movement. A page of
            // placeholders starts everything at once, so the continuation after a disabled animation
            // can still come back tens of milliseconds late; measured against a 32 ms probe that read
            // as animating and the placeholders kept moving with animations switched off.
            const int Length = 250;

            var elapsed = System.Diagnostics.Stopwatch.StartNew();
            await view.FadeTo(view.Opacity, Length);

            return elapsed.ElapsedMilliseconds >= Length / 2;
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
