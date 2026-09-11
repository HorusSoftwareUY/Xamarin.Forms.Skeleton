using System;

#if NET6_0_OR_GREATER
using Maui.Skeleton.Animations;
#else
using Xamarin.Forms.Skeleton.Animations;
using Xamarin.Forms.Xaml;
#endif

#if NET6_0_OR_GREATER
namespace Maui.Skeleton
#else
namespace Xamarin.Forms.Skeleton
#endif
{
    [ContentProperty(nameof(Source))]
    public sealed class DefaultAnimationExtension : IMarkupExtension<BaseAnimation>
    {
        /// <summary>
        /// Duration in milliseconds. Left unset each animation uses its own default, which is 500 for
        /// the four that animate a property of the view and longer for the three that repaint it.
        /// </summary>
        public int? Interval { get; set; }

        /// <summary>Interval the four original animations have always used when none is given.</summary>
        const int LegacyInterval = 500;

        public double? Parameter { get; set; }

        public AnimationTypes Source { get; set; }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Axis a sweeping animation travels along. Left unset each animation uses its own default,
        /// which is not the same for all of them. Ignored by animations that do not sweep.
        /// </summary>
        public SweepAxis? Direction { get; set; }

        /// <summary>
        /// Two or three colours, written as <c>"#05FFFFFF,#24FFFFFF,#05FFFFFF"</c>, for the
        /// animations that repaint the placeholder. <c>Shimmer</c> and <c>Aurora</c> use all of them
        /// as the gradient they move; <c>Tint</c> uses the middle one, the colour it washes to.
        /// Ignored by the animations that instead animate a property of the view.
        /// </summary>
        /// <remarks>
        /// A markup extension separates its properties with commas, so this value has to be quoted:
        /// <c>SweepColors='#05FFFFFF,#24FFFFFF'</c>.
        /// </remarks>
        [System.ComponentModel.TypeConverter(typeof(SweepColorsTypeConverter))]
        public Color[]? SweepColors { get; set; }
#endif

        public BaseAnimation ProvideValue(IServiceProvider serviceProvider)
        {
            switch (Source)
            {
                case AnimationTypes.Beat:
                    return new BeatAnimation(Interval ?? LegacyInterval, Parameter);
                case AnimationTypes.Fade:
                    return new FadeAnimation(Interval ?? LegacyInterval, Parameter);
                case AnimationTypes.VerticalShake:
                    return new VerticalShakeAnimation(Interval ?? LegacyInterval, Parameter);
                case AnimationTypes.HorizontalShake:
                    return new HorizontalShakeAnimation(Interval ?? LegacyInterval, Parameter);
#if NET6_0_OR_GREATER
                case AnimationTypes.Shimmer:
                    return new ShimmerAnimation(Interval, Direction, SweepColors);
                case AnimationTypes.Aurora:
                    return new AuroraAnimation(Interval, Direction, SweepColors);
                case AnimationTypes.Tint:
                    return new TintAnimation(Interval, SweepColors);
#endif
                case AnimationTypes.None:
                default:
                    return null;
            }
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);
    }
}