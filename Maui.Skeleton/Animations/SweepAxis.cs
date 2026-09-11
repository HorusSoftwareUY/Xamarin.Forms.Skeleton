namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// The axis a sweeping animation travels along. Shared by every animation that moves a band of
    /// colour across the placeholder rather than changing a property of the view as a whole.
    ///
    /// Named SweepAxis rather than SweepDirection because Microsoft.Maui.Controls already has a
    /// SweepDirection, for arc segments, and that namespace is in scope in every MAUI file.
    /// </summary>
    public enum SweepAxis
    {
        /// <summary>Left to right.</summary>
        Horizontal,

        /// <summary>Top to bottom.</summary>
        Vertical,

        /// <summary>Top left to bottom right.</summary>
        Diagonal,

        /// <summary>Top right to bottom left.</summary>
        DiagonalReverse
    }
}
