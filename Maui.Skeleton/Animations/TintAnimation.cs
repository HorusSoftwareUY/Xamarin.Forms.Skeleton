namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Washes a colour over the placeholder and back out again.
    /// </summary>
    /// <remarks>
    /// See <see cref="BackgroundAnimation"/> for what every animation that repaints the placeholder
    /// has in common, including why this cannot work on a <c>Frame</c>.
    ///
    /// The quietest of the three. Nothing moves and nothing has a shape: the whole placeholder
    /// simply takes on a colour and lets it go. Where Shimmer draws the eye to a band travelling
    /// past, this only registers as the surface having changed, which is what to reach for when the
    /// loading state should stay in the background.
    ///
    /// It takes a single colour, the peak of <see cref="BackgroundAnimation.SweepColors"/>, so the
    /// same palette can be handed to any of the three. <see cref="BaseAnimation.Interval"/> is the
    /// whole movement, in and out.
    /// </remarks>
    public class TintAnimation : BackgroundAnimation
    {
        /// <summary>Duration of one full wash, in and out. Matches the simulator's default.</summary>
        public const int DefaultInterval = 1600;

        /// <summary>
        /// White at 18%, enough to read as a wash on a dark placeholder without becoming a shape.
        /// Only the middle entry is used; the outer two are there so the same palette works for the
        /// sweeping animations too.
        /// </summary>
        static readonly Color[] LightWash =
        [
            Color.FromArgb("#00FFFFFF"),
            Color.FromArgb("#2EFFFFFF"),
            Color.FromArgb("#00FFFFFF")
        ];

        /// <summary>The same wash in black, for a light placeholder.</summary>
        static readonly Color[] DarkWash =
        [
            Color.FromArgb("#00000000"),
            Color.FromArgb("#2E000000"),
            Color.FromArgb("#00000000")
        ];

        public TintAnimation() => Interval = DefaultInterval;

        public TintAnimation(int? interval, Color[]? sweepColors)
        {
            Interval = Duration(interval, DefaultInterval);
            SweepColors = sweepColors;
        }

        protected override Color[] DefaultColorsFor(Color placeholder) =>
            Luminance(placeholder) < 0.5 ? LightWash : DarkWash;

        /// <summary>
        /// Resolves the two ends of the wash once per pass. Only the peak of the palette is used:
        /// there is no gradient here for the other two colours to sit in.
        /// </summary>
        protected override Color[] Prepare(Color placeholder) =>
            [placeholder, Over(Normalise(SweepColors ?? DefaultColorsFor(placeholder))[1], placeholder)];

        /// <summary>
        /// In over the first half of the pass and out over the second, easing at both ends and at the
        /// turn. The reference animates opacity from 0 to 1 and back with ease-in-out between each
        /// pair of keyframes, which is what the cosine reproduces without a bezier solver.
        /// </summary>
        protected override Brush BrushAt(Color[] colors, double progress)
        {
            var leg = progress < 0.5 ? progress * 2 : (1 - progress) * 2;
            var amount = (1 - Math.Cos(Math.PI * leg)) / 2;

            return new SolidColorBrush(Blend(colors[0], colors[1], amount));
        }

        static Color Blend(Color from, Color to, double amount) => new(
            (float)(from.Red + (to.Red - from.Red) * amount),
            (float)(from.Green + (to.Green - from.Green) * amount),
            (float)(from.Blue + (to.Blue - from.Blue) * amount),
            from.Alpha);
    }
}
