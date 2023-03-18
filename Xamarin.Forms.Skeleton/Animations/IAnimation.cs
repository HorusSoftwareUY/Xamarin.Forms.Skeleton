#if NET6_0_OR_GREATER
namespace Maui.Skeleton.Animations
#else
namespace Xamarin.Forms.Skeleton.Animations
#endif
{
    public interface IAnimation
    {
        void Start(BindableObject bindable);
        void Stop(BindableObject bindable);
    }
}
