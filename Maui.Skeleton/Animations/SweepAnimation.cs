namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Shared machinery for the animations that move a gradient across the placeholder rather than
    /// changing a property of the view.
    /// </summary>
    /// <remarks>
    /// These paint the view's <see cref="VisualElement.Background"/> and slide what is painted, which
    /// is what separates them from Fade, Beat and the shakes. Three consequences follow, and they are
    /// the same for every animation built on this:
    ///
    /// The animation has to be attached to the element that shows the placeholder colour. It is that
    /// element's own background, so it does not carry down to children the way Fade does, and on a
    /// transparent container it paints something nobody can see.
    ///
    /// It does not work on <c>Frame</c>. That control is deprecated in MAUI and its renderer does not
    /// repaint when the background is replaced, so the gradient freezes where it was first drawn.
    /// <c>Border</c> repaints correctly.
    ///
    /// <see cref="BaseAnimation.Parameter"/> has no meaning. The look comes from
    /// <see cref="SweepColors"/> and <see cref="Direction"/>.
    /// </remarks>
    public abstract class SweepAnimation : BaseAnimation
    {
        /// <summary>
        /// Milliseconds between frames. 30fps rather than 60: these move slowly enough that 60 does
        /// not read as smoother, and a list of placeholders animates many views at once.
        /// </summary>
        protected const uint FrameRate = 33;

        /// <summary>The axis the gradient travels along.</summary>
        public SweepAxis Direction { get; set; } = SweepAxis.Horizontal;

        /// <summary>
        /// Two or three colours for the gradient. Two are read as edge and peak and mirrored, three
        /// are taken as written, which is how the two ends get different colours. Null falls back to
        /// <see cref="DefaultColorsFor"/>.
        /// </summary>
        /// <remarks>
        /// Each colour's alpha is composited over the placeholder, so these describe light falling on
        /// it rather than replacing it. Written as <c>#AARRGGBB</c>, the order MAUI reads; CSS writes
        /// the same colour as <c>#RRGGBBAA</c>.
        /// </remarks>
        [System.ComponentModel.TypeConverter(typeof(SweepColorsTypeConverter))]
        public Color[]? SweepColors { get; set; }

        /// <summary>Where each colour sits inside the gradient, from its start to its end.</summary>
        protected abstract float[] Stops { get; }

        /// <summary>Colours to use when <see cref="SweepColors"/> was not set.</summary>
        protected abstract Color[] DefaultColorsFor(Color placeholder);

        /// <summary>
        /// Where the gradient sits at this point of the pass, measured along the sweep axis in
        /// multiples of the element: 0 is its leading edge, 1 its trailing edge.
        /// </summary>
        protected abstract (double From, double To) ExtentAt(double progress);

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            if (bindable is not View view)
                return false;

            // Skeleton applies the placeholder colour before starting the animation, so this is the
            // colour the gradient travels over. It only changes between passes, so the composited
            // colours are resolved here and only the geometry is rebuilt per frame.
            var placeholder = view.BackgroundColor ?? Colors.Transparent;
            var colors = Resolve(placeholder);

            // Phase comes from a clock shared by every view, not from when this particular pass
            // started. Each view running its own stopwatch drifts, and a page full of placeholders
            // ends up with every gradient in a different place, where the reference has them move
            // together. Reading the phase from a process wide clock keeps them locked without any
            // coordination between the animations.
            //
            // The pass still lasts a full interval of wall time. That keeps it honest for the guard
            // in BaseAnimation.Run, which stops an animation that returns faster than a frame, and it
            // means a busy UI thread drops frames rather than stretching the movement.
            var interval = Math.Max(1u, Interval);
            var began = Environment.TickCount64;

            while (!Skeleton.GetCancelAnimation(view))
            {
                var now = Environment.TickCount64;

                view.Background = BuildBrush(colors, now % interval / (double)interval);

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

        /// <summary>
        /// Composites the gradient's colours over the placeholder, once per pass. The outermost
        /// entries are the placeholder itself, so the gradient pads out to it on both sides instead
        /// of smearing its own colour across the rest of the view.
        /// </summary>
        Color[] Resolve(Color placeholder)
        {
            var sweep = Normalise(SweepColors ?? DefaultColorsFor(placeholder));

            return
            [
                placeholder,
                Over(sweep[0], placeholder),
                Over(sweep[1], placeholder),
                Over(sweep[2], placeholder),
                placeholder
            ];
        }

        /// <summary>
        /// Builds the gradient at a point in its travel.
        /// </summary>
        /// <remarks>
        /// A fresh brush per frame, rather than moving the endpoints of one: mutating a brush already
        /// assigned to <see cref="VisualElement.Background"/> does not repaint, so the gradient sits
        /// wherever it was first drawn. Assigning a new brush does repaint. The colours are already
        /// resolved, so this allocates the brush and its stops and nothing else.
        /// </remarks>
        LinearGradientBrush BuildBrush(Color[] colors, double progress)
        {
            var (from, to) = ExtentAt(progress);

            var (start, end) = Direction switch
            {
                SweepAxis.Horizontal => (new Point(from, 0.5), new Point(to, 0.5)),
                SweepAxis.Vertical => (new Point(0.5, from), new Point(0.5, to)),
                SweepAxis.DiagonalReverse => (new Point(1 - from, from), new Point(1 - to, to)),
                _ => (new Point(from, from), new Point(to, to))
            };

            var stops = Stops;
            var collection = new GradientStopCollection();
            for (var i = 0; i < colors.Length; i++)
                collection.Add(new GradientStop(colors[i], stops[i]));

            return new LinearGradientBrush { StartPoint = start, EndPoint = end, GradientStops = collection };
        }

        /// <summary>Expands the two colour form, edge and peak, into the three the gradient needs.</summary>
        static Color[] Normalise(Color[] colors) => colors.Length switch
        {
            2 => [colors[0], colors[1], colors[0]],
            3 => colors,
            _ => throw new ArgumentException(
                $"A sweep takes two or three colours, got {colors.Length}.", nameof(colors))
        };

        /// <summary>
        /// Composites a sweep colour over the placeholder. The gradient replaces the view's
        /// background rather than sitting on top of it, so the translucency is resolved here.
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
