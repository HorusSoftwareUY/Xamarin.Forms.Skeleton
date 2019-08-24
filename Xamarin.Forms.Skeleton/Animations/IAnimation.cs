using System.Threading.Tasks;

namespace Xamarin.Forms.Skeleton.Animations
{
    public interface IAnimation
    {
        void Start(BindableObject bindable);
        void Stop(BindableObject bindable);
    }
}
