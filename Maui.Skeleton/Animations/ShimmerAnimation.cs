namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Sweeps a band of light across the placeholder, the effect most loading skeletons use.
    /// </summary>
    /// <remarks>
    /// See <see cref="SweepAnimation"/> for what every sweeping animation has in common, including
    /// why this has to go on the element that shows the placeholder colour and why it cannot work on
    /// a <c>Frame</c>.
    ///
    /// <see cref="BaseAnimation.Interval"/> is the duration of the whole pass. The band only travels
    /// one way, so unlike Fade or Beat there is no second half to come back on.
    /// </remarks>
    public class ShimmerAnimation : SweepAnimation
    {
        /// <summary>Duration of one pass. Matches the default of the animation simulator.</summary>
        public const int DefaultInterval = 1600;

        /// <summary>
        /// Width of the band, as a fraction of the element it travels over. The reference sweeps a
        /// gradient exactly as wide as the element, which is what makes the light read as a soft wash
        /// rather than a hard stripe; a narrow band looks like a line crossing the shape.
        /// </summary>
        const double BandWidth = 1.0;

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

        /// <summary>The reference sweeps a shimmer left to right.</summary>
        const SweepAxis DefaultDirection = SweepAxis.Horizontal;

        public ShimmerAnimation()
        {
            Interval = DefaultInterval;
            Direction = DefaultDirection;
        }

        public ShimmerAnimation(int interval, SweepAxis? direction, Color[]? sweepColors)
        {
            Interval = (uint)interval;
            Direction = direction ?? DefaultDirection;
            SweepColors = sweepColors;
        }

        /// <summary>
        /// 35 / 50 / 65 rather than evenly spaced, so the light ramps up over the first third, peaks
        /// in the middle and falls away over the last third, as the reference gradient does.
        /// </summary>
        protected override float[] Stops => [0f, 0.35f, 0.5f, 0.65f, 1f];

        /// <summary>
        /// Picks the band that will actually be visible. A white sweep over a light grey placeholder
        /// is very nearly invisible, and a shimmer nobody can see is the worst thing this animation
        /// can do, so the default follows the placeholder instead of being a fixed colour.
        /// </summary>
        protected override Color[] DefaultColorsFor(Color placeholder) =>
            Luminance(placeholder) < 0.5 ? LightSweep : DarkSweep;

        /// <summary>
        /// The band starts just off one edge and finishes just off the other, so it enters and leaves
        /// cleanly rather than appearing mid-view.
        /// </summary>
        protected override (double From, double To) ExtentAt(double progress)
        {
            var from = -BandWidth + Ease(progress) * (1 + BandWidth);
            return (from, from + BandWidth);
        }

        /// <summary>
        /// cubic-bezier(.25, 1, .5, 1), the curve the reference sweep uses. It front-loads the travel
        /// heavily: a fifth of the way through the pass the band has already covered 60% of the
        /// distance, against 49% for a plain cubic ease-out. That difference is what makes the sweep
        /// read as quick rather than as a slow drift, so it is worth solving the curve properly
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
    }
}
