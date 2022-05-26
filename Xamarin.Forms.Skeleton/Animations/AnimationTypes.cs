#if NET6_0_OR_GREATER
namespace Maui.Skeleton.Animations
#else
namespace Xamarin.Forms.Skeleton.Animations
#endif
{
    public enum AnimationTypes
    {
        None,
        Beat,
        Fade,
        VerticalShake,
        HorizontalShake
    }
}
