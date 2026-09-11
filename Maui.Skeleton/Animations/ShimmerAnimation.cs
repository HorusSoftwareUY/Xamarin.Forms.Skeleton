namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Sweeps a band of light across the placeholder, the effect most loading skeletons use.
    /// </summary>
    /// <remarks>
    /// Unlike the other built-ins, this one does not animate a property of the view. It paints the
    /// view's <see cref="VisualElement.Background"/> with a gradient and slides that gradient across,
    /// which is why <see cref="BaseAnimation.Parameter"/> has no meaning here: the look comes from
    /// <see cref="SweepColors"/> and <see cref="Direction"/> instead.
    ///
    /// <see cref="BaseAnimation.Interval"/> is also read differently. For Fade or Beat it is half a
    /// cycle, because those animate out and back. A sweep only travels one way, so here it is the
    /// duration of the whole pass.
    ///
    /// Because the effect is the view's own background, it has to be attached to the element that
    /// shows the placeholder colour. Putting it on a transparent container does nothing visible, and
    /// unlike Fade or Beat it does not carry down to the children.
    /// </remarks>
    public class ShimmerAnimation : BaseAnimation
    {
        /// <summary>Duration of one pass. Matches the default of the animation simulator.</summary>
        public const int DefaultInterval = 1600;


        /// <summary>
        /// Width of the band, as a fraction of the element it travels over. The reference sweeps a
        /// gradient exactly as wide as the element, which is what makes the light read as a soft
        /// wash rather than a hard stripe; a narrow band looks like a line crossing the shape.
        /// </summary>
        const double BandWidth = 1.0;

        /// <summary>
        /// Milliseconds between frames. 30fps rather than 60: a sweep this slow does not read as
        /// smoother at 60, and a list of placeholders animates many views at once.
        /// </summary>
        const uint FrameRate = 33;

        /// <summary>
        /// White at 2% along the edges and 14% at the peak, for a dark placeholder. Subtle enough to
        /// read as light moving over the placeholder rather than as a second shape on top of it.
        /// </summary>
        static readonly Color[] LightSweep =
        [
            Color.FromArgb("#05FFFFFF"),
            Color.FromArgb("#24FFFFFF"),
            Color.FromArgb("#05FFFFFF")
        ];

        /// <summary>The same band in black, for a light placeholder.</summary>
        static readonly Color[] DarkSweep =
        [
            Color.FromArgb("#05000000"),
            Color.FromArgb("#24000000"),
            Color.FromArgb("#05000000")
        ];

        /// <summary>
        /// Picks the band that will actually be visible. A white sweep over a light grey placeholder
        /// is very nearly invisible, and a shimmer nobody can see is the worst thing this animation
        /// can do, so the default follows the placeholder instead of being a fixed colour. Set
        /// <see cref="SweepColors"/> to override it.
        /// </summary>
        static Color[] DefaultFor(Color placeholder) =>
            Luminance(placeholder) < 0.5 ? LightSweep : DarkSweep;

        /// <summary>Relative luminance, the sRGB coefficients used by WCAG.</summary>
        static double Luminance(Color color) =>
            0.2126 * color.Red + 0.7152 * color.Green + 0.0722 * color.Blue;

        public ShimmerAnimation() => Interval = DefaultInterval;

        public ShimmerAnimation(int interval, SweepAxis direction, Color[]? sweepColors)
        {
            Interval = (uint)interval;
            Direction = direction;
            SweepColors = sweepColors;
        }

        /// <summary>The axis the band travels along. Defaults to <see cref="SweepAxis.Horizontal"/>.</summary>
        public SweepAxis Direction { get; set; } = SweepAxis.Horizontal;

        /// <summary>
        /// Two or three colours for the band. Two are read as edge and peak and mirrored, so
        /// <c>"#05FFFFFF,#24FFFFFF"</c> and <c>"#05FFFFFF,#24FFFFFF,#05FFFFFF"</c> describe the same
        /// sweep. Three are taken as written, which is how a sweep gets different colours at its two
        /// ends. Null picks a band that contrasts with the placeholder: light over a dark
        /// placeholder, dark over a light one.
        /// </summary>
        /// <remarks>
        /// The alpha of each colour is composited over the placeholder colour, so these describe
        /// light falling on the placeholder rather than replacing it.
        /// </remarks>
        [System.ComponentModel.TypeConverter(typeof(SweepColorsTypeConverter))]
        public Color[]? SweepColors { get; set; }

        protected override async Task<bool> Animate(BindableObject bindable)
        {
            if (bindable is not View view)
                return false;

            // Skeleton applies the placeholder colour before starting the animation, so this is the
            // colour the band travels over. It only changes between passes, so the composited
            // colours are resolved here and only the geometry is rebuilt per frame.
            var placeholder = view.BackgroundColor ?? Colors.Transparent;
            var colors = Resolve(placeholder);

            // Phase comes from a clock shared by every view, not from when this particular pass
            // started. Each view running its own stopwatch drifts, and a page full of placeholders
            // ends up with every band in a different place, where the reference has them sweep
            // together. Reading the phase from a process wide clock keeps them locked without any
            // coordination between the animations.
            //
            // The pass still lasts a full interval of wall time. That keeps it honest for the guard
            // in BaseAnimation.Run, which stops an animation that returns faster than a frame, and
            // it means a busy UI thread drops frames rather than stretching the sweep.
            var interval = Math.Max(1u, Interval);
            var began = Environment.TickCount64;

            while (!Skeleton.GetCancelAnimation(view))
            {
                var now = Environment.TickCount64;

                view.Background = BuildBrush(colors, Ease(now % interval / (double)interval), Direction);

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
        /// Waits one frame on the view's own dispatcher.
        /// </summary>
        static Task NextFrame(View view)
        {
            var next = new TaskCompletionSource<bool>();
            view.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(FrameRate), () => next.TrySetResult(true));
            return next.Task;
        }

        /// <summary>
        /// cubic-bezier(.25, 1, .5, 1), the curve the reference sweep uses. It front-loads the
        /// travel heavily: a fifth of the way through the pass the band has already covered 60% of
        /// the distance, against 49% for a plain cubic ease-out. That difference is what makes the
        /// sweep read as quick rather than as a slow drift, so it is worth solving the curve properly
        /// instead of approximating it.
        /// </summary>
        /// <remarks>Newton-Raphson on the x polynomial, as WebKit's UnitBezier does it.</remarks>
        static double Ease(double t)
        {
            const double cx = 3 * 0.25;
            const double bx = 3 * (0.5 - 0.25) - cx;
            const double ax = 1 - cx - bx;

            var u = t;
            for (var i = 0; i < 8; i++)
            {
                var x = ((ax * u + bx) * u + cx) * u - t;
                if (Math.Abs(x) < 1e-5)
                    break;

                var slope = (3 * ax * u + 2 * bx) * u + cx;
                if (Math.Abs(slope) < 1e-6)
                    break;

                u -= x / slope;
            }

            // With both control points at y = 1 the y polynomial reduces to 1 - (1 - u)^3.
            var inverse = 1 - u;
            return 1 - inverse * inverse * inverse;
        }

        /// <summary>
        /// Composites the band's colours over the placeholder. The gradient replaces the view's
        /// background rather than sitting on top of it, so the translucency is resolved here.
        /// </summary>
        Color[] Resolve(Color placeholder)
        {
            var sweep = Normalise(SweepColors ?? DefaultFor(placeholder));

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
        /// Builds the band at a point in its travel. The endpoints start just off one edge and finish
        /// just off the other, so the band enters and leaves cleanly rather than appearing mid-view.
        /// The outermost stops are the placeholder colour, so the gradient, which spans only the band
        /// itself, pads out to the placeholder instead of smearing the band's colour across the rest.
        /// </summary>
        /// <remarks>
        /// A fresh brush per frame, rather than moving the endpoints of one: mutating a brush already
        /// assigned to <see cref="VisualElement.Background"/> does not repaint, so the band sits
        /// wherever it was first drawn. Assigning a new brush does repaint. The colours are already
        /// resolved, so this allocates the brush and its stops and nothing else.
        /// </remarks>
        static LinearGradientBrush BuildBrush(Color[] colors, double progress, SweepAxis direction)
        {
            const double half = BandWidth / 2;

            // Centre of the band, travelling from fully before the view to fully past it.
            var centre = -half + progress * (1 + BandWidth);
            var from = centre - half;
            var to = centre + half;

            var (start, end) = direction switch
            {
                SweepAxis.Horizontal => (new Point(from, 0.5), new Point(to, 0.5)),
                SweepAxis.Vertical => (new Point(0.5, from), new Point(0.5, to)),
                SweepAxis.DiagonalReverse => (new Point(1 - from, from), new Point(1 - to, to)),
                _ => (new Point(from, from), new Point(to, to))
            };

            return new LinearGradientBrush
            {
                StartPoint = start,
                EndPoint = end,
                GradientStops =
                [
                    // 35 / 50 / 65 rather than evenly spaced, so the light ramps up over the first
                    // third, peaks in the middle and falls away over the last third, as the
                    // reference gradient does.
                    new GradientStop(colors[0], 0f),
                    new GradientStop(colors[1], 0.35f),
                    new GradientStop(colors[2], 0.5f),
                    new GradientStop(colors[3], 0.65f),
                    new GradientStop(colors[4], 1f)
                ]
            };
        }

        /// <summary>Expands the two colour form, edge and peak, into the three the gradient needs.</summary>
        static Color[] Normalise(Color[] colors) => colors.Length switch
        {
            2 => [colors[0], colors[1], colors[0]],
            3 => colors,
            _ => throw new ArgumentException(
                $"A sweep takes two or three colours, got {colors.Length}.", nameof(colors))
        };

        static Color Over(Color light, Color placeholder)
        {
            var alpha = light.Alpha;

            return new Color(
                light.Red * alpha + placeholder.Red * (1 - alpha),
                light.Green * alpha + placeholder.Green * (1 - alpha),
                light.Blue * alpha + placeholder.Blue * (1 - alpha),
                placeholder.Alpha);
        }
    }
}
