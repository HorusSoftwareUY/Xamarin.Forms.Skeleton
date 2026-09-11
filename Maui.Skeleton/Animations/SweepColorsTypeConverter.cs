using System.ComponentModel;
using System.Globalization;

namespace Maui.Skeleton.Animations
{
    /// <summary>
    /// Reads the comma separated form a sweep's colours take in XAML,
    /// <c>"#05FFFFFF,#24FFFFFF,#05FFFFFF"</c>, into the colours themselves.
    /// </summary>
    /// <remarks>
    /// Each colour goes through <see cref="Color.FromArgb(string)"/>, so the hexadecimal is read the
    /// way the rest of MAUI reads it: eight digits are <c>#AARRGGBB</c>, alpha first. CSS writes the
    /// same colour as <c>#RRGGBBAA</c>, so a value copied from a stylesheet has to be reordered.
    /// </remarks>
    public sealed class SweepColorsTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
            sourceType == typeof(string);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
        {
            if (value is not string text || string.IsNullOrWhiteSpace(text))
                return null;

            // Empty entries are kept on purpose. Dropping them turns "#05FFFFFF,,#24FFFFFF", where
            // someone meant three colours and typed one comma too many, into a valid two colour
            // palette that is silently mirrored instead of reported.
            var parts = text.Split(',', StringSplitOptions.TrimEntries);

            // Checked here rather than where the colours are used. Past this point the animation runs
            // on a fire and forget task whose exceptions BaseAnimation only writes to debug output,
            // so a malformed value would surface as a placeholder that silently never moves.
            if (parts.Length is not (2 or 3))
                throw new FormatException(
                    $"Sweep colours take two or three values, got {parts.Length}: '{text}'. Two are read as edge and peak and mirrored, three as written.");

            var colors = new Color[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                colors[i] = Color.FromArgb(parts[i])
                    ?? throw new FormatException($"'{parts[i]}' is not a colour. Sweep colours are written as #AARRGGBB, for example #24FFFFFF.");
            }

            return colors;
        }
    }
}
