using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Skeleton.Animations;

namespace Xamarin.Forms.Skeleton
{
    public static class Skeleton
    {
        //private double DefaultScaleTo { get; set; }
        //private int DefaultAnimationTime { get; set; }
        //public double ScaleTo { get; set; }
        //public int AnimationTime { get; set; }
        //public bool CancelAnimation { get; set; }

        //public bool Animating { get; set; }
        //public int Timeout { get; set; } //seconds
        //public DateTime StartTime { get; set; }

        public static readonly BindableProperty AnimationProperty = BindableProperty.CreateAttached("Animation", typeof(AnimationsPack), typeof(Layout), AnimationsPack.None);

        public static void SetAnimation(BindableObject b, AnimationsPack value)
        {
            b.SetValue(AnimationProperty, value);
        }

        public static AnimationsPack GetAnimation(BindableObject b)
        {
            return (AnimationsPack)b.GetValue(AnimationProperty);
        }

        public static readonly BindableProperty IsBusyProperty = BindableProperty.CreateAttached("IsBusy", typeof(bool), typeof(Layout), default(bool), propertyChanged: (b, oldValue, newValue) => OnIsBusyChanged(b, (bool)oldValue, (bool)newValue));

        public static void SetIsBusy(BindableObject b, bool value)
        {
            b.SetValue(IsBusyProperty, value);
        }

        public static bool GetIsBusy(BindableObject b)
        {
            return (bool)b.GetValue(IsBusyProperty);
        }

        static void OnIsBusyChanged(BindableObject bindable, bool oldValue, bool newValue)
        {
            if (bindable.GetType().IsSubclassOf(typeof(Layout)))
                HandleAnimation(bindable, newValue);
            else
                throw new NotSupportedException();
        }

        public static readonly BindableProperty AnimatingProperty = BindableProperty.CreateAttached("Animating", typeof(bool), typeof(Layout), default(bool));

        public static void SetAnimating(BindableObject b, bool value)
        {
            b.SetValue(AnimatingProperty, value);
        }

        public static bool GetAnimating(BindableObject b)
        {
            return (bool)b.GetValue(AnimatingProperty);
        }

        internal static readonly BindableProperty CancelAnimationProperty = BindableProperty.CreateAttached("CancelAnimation", typeof(bool), typeof(Layout), default(bool));

        internal static void SetCancelAnimation(BindableObject b, bool value)
        {
            b.SetValue(CancelAnimationProperty, value);
        }

        internal static bool GetCancelAnimation(BindableObject b)
        {
            return (bool)b.GetValue(CancelAnimationProperty);
        }

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(Layout), default(Color));

        public static void SetBackgroundColor(BindableObject b, Color value)
        {
            b.SetValue(BackgroundColorProperty, value);
        }

        public static Color GetBackgroundColor(BindableObject b)
        {
            return (Color)b.GetValue(BackgroundColorProperty);
        }

        public static readonly BindableProperty OriginalBackgroundColorProperty = BindableProperty.CreateAttached("OriginalBackgroundColor", typeof(Color), typeof(Layout), default(Color));

        public static void SetOriginalBackgroundColor(BindableObject b, Color value)
        {
            b.SetValue(OriginalBackgroundColorProperty, value);
        }

        public static Color GetOriginalBackgroundColor(BindableObject b)
        {
            return (Color)b.GetValue(OriginalBackgroundColorProperty);
        }

        public static bool IsSkeleton(BindableObject bindable)
        {
            return GetBackgroundColor(bindable) != default(Color) || GetIsBusy(bindable);
        }

        static void HandleAnimation(BindableObject bindable, bool newValue)         {             if (!(bindable is Layout))
                return;

            AnimationsPack animationType = GetAnimation(bindable);
            Layout self = (Layout)bindable;             if (newValue)
            {
                if (self.Children != null && self.Children.Count > 0)
                    self.Children.ToList().ForEach(i =>
                    {
                        HandleChildOnRun(i);
                    });

                SetOriginalBackgroundColor(bindable, self.BackgroundColor);
                self.BackgroundColor = GetBackgroundColor(bindable) != default(Color) ? GetBackgroundColor(bindable) : Color.FromHex("1Ab2b2b2");

                if (animationType == AnimationsPack.None || GetAnimating(bindable))
                    return;
                 SetCancelAnimation(bindable, false);                 RunCustomAnimation(bindable, animationType);             }             else  //stop
            {
                if (animationType != AnimationsPack.None)
                    SetCancelAnimation(bindable, true);

                self.BackgroundColor = GetOriginalBackgroundColor(bindable) != default(Color) ? GetOriginalBackgroundColor(bindable) : Color.Transparent;
                self.Children.ToList().ForEach(i =>
                {
                    HandleChildOnStop(i);
                });
            }         }

        private static void HandleChildOnStop(Element i)
        {
            if (i is VisualElement)
            {
                VisualElement child = (VisualElement)i;
                child.SetValue(VisualElement.IsVisibleProperty, true);
            }
        }

        private static void HandleChildOnRun(Element i)
        {
            if (i is VisualElement)
            {
                VisualElement child = (VisualElement)i;
                child.SetValue(VisualElement.IsVisibleProperty, false);

                //if (IsSkeleton(i))
                //{
                //    child.SetValue(VisualElement.IsVisibleProperty, true);
                //    SetOriginalBackgroundColor(child, child.BackgroundColor);
                //    child.BackgroundColor = GetBackgroundColor(child);
                //}
                //else
                //{
                //    child.SetValue(VisualElement.IsVisibleProperty, false);
                //}
            }
        }

        private static void RunCustomAnimation(BindableObject bindable, AnimationsPack animationType)         {
            switch (animationType)
            {
                case AnimationsPack.Beat:
                    new BeatAnimation().Start(bindable);
                    break;
                case AnimationsPack.Fade:
                    new FadeAnimation().Start(bindable);
                    break;
                case AnimationsPack.VerticalShake:
                    new VerticalShakeAnimation().Start(bindable);
                    break;
                case AnimationsPack.HorizontalShake:
                    new HorizontalShakeAnimation().Start(bindable);
                    break;
                default:
                    break;
            }         }
    }
}
