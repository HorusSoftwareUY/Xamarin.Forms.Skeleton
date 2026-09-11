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

            var parts = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

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
