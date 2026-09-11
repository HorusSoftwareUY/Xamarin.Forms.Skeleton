namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Drifts a wide, soft field of colour back and forth across the placeholder.
    /// </summary>
    /// <remarks>
    /// See <see cref="SweepAnimation"/> for what every sweeping animation has in common, including
    /// why this has to go on the element that shows the placeholder colour and why it cannot work on
    /// a <c>Frame</c>.
    ///
    /// Where Shimmer sends a band the width of the element across it and off the other side, this
    /// holds a gradient far larger than the element and pans a window over it, so colour is always
    /// present and only shifts. That is the difference between a sweep you notice passing and a
    /// slow wash you notice having changed.
    ///
    /// <see cref="BaseAnimation.Interval"/> is the whole movement, out and back, matching the
    /// reference. Note this is not how Fade and Beat read it: for those it is half a cycle.
    /// </remarks>
    public class AuroraAnimation : SweepAnimation
    {
        /// <summary>Duration of one full drift, out and back. Matches the simulator's default.</summary>
        public const int DefaultInterval = 1600;

        /// <summary>
        /// How far the gradient reaches, in multiples of the element. The reference sets
        /// <c>background-size: 260%</c>, so only part of the gradient is ever on screen and panning
        /// the window over it is what produces the movement.
        /// </summary>
        const double Scale = 2.6;

        /// <summary>
        /// Enough colour to read as a wash without turning the placeholder into a second shape. These
        /// are the neutral fallback; the point of Aurora is the palette, so most uses will set
        /// <see cref="SweepAnimation.SweepColors"/>.
        /// </summary>
        static readonly Color[] LightDrift =
        [
            Color.FromArgb("#0AFFFFFF"),
            Color.FromArgb("#2EFFFFFF"),
            Color.FromArgb("#0AFFFFFF")
        ];

        /// <summary>The same drift in black, for a light placeholder.</summary>
        static readonly Color[] DarkDrift =
        [
            Color.FromArgb("#0A000000"),
            Color.FromArgb("#2E000000"),
            Color.FromArgb("#0A000000")
        ];

        /// <summary>
        /// The reference pans horizontally: its keyframes move background-position from 0 to 100%
        /// while the vertical component stays at 50%. The gradient itself is drawn at 115 degrees,
        /// but that is the angle of the colour bands, not the direction they travel, and this
        /// animation has one axis for both.
        /// </summary>
        const SweepAxis DefaultDirection = SweepAxis.Horizontal;

        public AuroraAnimation()
        {
            Interval = DefaultInterval;
            Direction = DefaultDirection;
        }

        public AuroraAnimation(int? interval, SweepAxis? direction, Color[]? sweepColors)
        {
            Interval = (uint)(interval ?? DefaultInterval);
            Direction = direction ?? DefaultDirection;
            SweepColors = sweepColors;
        }

        /// <summary>
        /// 22 / 42 / 62 with the colour running out well before the end, so the gradient carries a
        /// long, soft tail rather than ending abruptly at its edge.
        /// </summary>
        protected override float[] Stops => [0f, 0.22f, 0.42f, 0.62f, 0.88f];

        protected override Color[] DefaultColorsFor(Color placeholder) =>
            Luminance(placeholder) < 0.5 ? LightDrift : DarkDrift;

        /// <summary>
        /// Pans the window over the gradient and back. The gradient never leaves the element, which
        /// is why colour is always visible, unlike Shimmer where the band spends part of the pass off
        /// screen.
        /// </summary>
        protected override (double From, double To) ExtentAt(double progress)
        {
            var from = -(Scale - 1) * PingPong(progress);
            return (from, from + Scale);
        }

        /// <summary>
        /// Out over the first half of the pass and back over the second, easing at both ends and at
        /// the turn. The reference applies ease-in-out between each pair of keyframes, which is what
        /// the cosine reproduces without a bezier solver.
        /// </summary>
        static double PingPong(double t)
        {
            var leg = t < 0.5 ? t * 2 : (1 - t) * 2;
            return (1 - Math.Cos(Math.PI * leg)) / 2;
        }
    }
}
