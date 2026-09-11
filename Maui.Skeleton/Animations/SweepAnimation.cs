namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Shared machinery for the animations that move a gradient across the placeholder.
    /// </summary>
    /// <remarks>
    /// See <see cref="BackgroundAnimation"/> for what every animation that repaints the placeholder
    /// has in common, including why these cannot work on a <c>Frame</c>.
    /// </remarks>
    public abstract class SweepAnimation : BackgroundAnimation
    {
        Color[] _colors = [];

        /// <summary>The axis the gradient travels along.</summary>
        public SweepAxis Direction { get; set; } = SweepAxis.Horizontal;

        /// <summary>Where each colour sits inside the gradient, from its start to its end.</summary>
        protected abstract float[] Stops { get; }

        /// <summary>
        /// Where the gradient sits at this point of the pass, measured along the sweep axis in
        /// multiples of the element: 0 is the element's leading edge, 1 its trailing edge.
        /// </summary>
        protected abstract (double From, double To) ExtentAt(double progress);

        /// <summary>
        /// Composites the gradient's colours over the placeholder. The outermost entries are the
        /// placeholder itself, so the gradient pads out to it on both sides instead of smearing its
        /// own colour across the rest of the view.
        /// </summary>
        protected override void Prepare(Color placeholder)
        {
            var sweep = Normalise(SweepColors ?? DefaultColorsFor(placeholder));

            _colors =
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
        protected override Brush BrushAt(double progress)
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
            for (var i = 0; i < _colors.Length; i++)
                collection.Add(new GradientStop(_colors[i], stops[i]));

            return new LinearGradientBrush { StartPoint = start, EndPoint = end, GradientStops = collection };
        }
    }
}
