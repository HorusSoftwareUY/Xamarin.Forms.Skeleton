using System;
using System.Linq;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Skeleton.Animations;
using Xamarin.Forms.Skeleton.Extensions;

namespace Xamarin.Forms.Skeleton
{
    public static class Skeleton
    {
        #region Public Properties

        public static readonly BindableProperty IsParentProperty = BindableProperty.CreateAttached("IsParent", typeof(bool), typeof(View), false);

        public static void SetIsParent(BindableObject b, bool value) => b.SetValue(IsParentProperty, value);

        public static bool GetIsParent(BindableObject b) => (bool)b.GetValue(IsParentProperty);

        public static readonly BindableProperty IsBusyProperty = BindableProperty.CreateAttached("IsBusy", typeof(bool), typeof(View), default(bool), propertyChanged: (b, oldValue, newValue) => OnIsBusyChanged(b, (bool)newValue));

        public static void SetIsBusy(BindableObject b, bool value) => b.SetValue(IsBusyProperty, value);

        public static bool GetIsBusy(BindableObject b) => (bool)b.GetValue(IsBusyProperty);

        public static readonly BindableProperty HideProperty = BindableProperty.CreateAttached("Hide", typeof(bool), typeof(View), default(bool));

        public static void SetHide(BindableObject b, bool value) => b.SetValue(HideProperty, value);

        public static bool GetHide(BindableObject b) => (bool)b.GetValue(HideProperty);

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(View), default(Color));

        public static void SetBackgroundColor(BindableObject b, Color value) => b.SetValue(BackgroundColorProperty, value);

        public static Color GetBackgroundColor(BindableObject b) => (Color)b.GetValue(BackgroundColorProperty);

        public static readonly BindableProperty AnimationProperty = BindableProperty.CreateAttached("Animation", typeof(AnimationTypes), typeof(View), AnimationTypes.None);

        public static void SetAnimation(BindableObject b, AnimationTypes value) => b.SetValue(AnimationProperty, value);

        public static AnimationTypes GetAnimation(BindableObject b) => (AnimationTypes)b.GetValue(AnimationProperty);

        public static readonly BindableProperty AnimationIntervalProperty = BindableProperty.CreateAttached("AnimationInterval", typeof(int), typeof(View), 500);

        public static void SetAnimationInterval(BindableObject b, int value) => b.SetValue(AnimationIntervalProperty, value);

        public static int GetAnimationInterval(BindableObject b) => (int)b.GetValue(AnimationIntervalProperty);

        public static readonly BindableProperty AnimationParameterProperty = BindableProperty.CreateAttached("AnimationParameter", typeof(double?), typeof(View), null);

        public static void SetAnimationParameter(BindableObject b, double? value) => b.SetValue(AnimationParameterProperty, value);

        public static double? GetAnimationParameter(BindableObject b) => (double?)b.GetValue(AnimationParameterProperty);

        #endregion Public Properties

        #region Internal Properties

        internal static readonly BindableProperty AnimatingProperty = BindableProperty.CreateAttached("Animating", typeof(bool), typeof(View), default(bool));

        internal static void SetAnimating(BindableObject b, bool value) => b.SetValue(AnimatingProperty, value);

        internal static bool GetAnimating(BindableObject b) => (bool)b.GetValue(AnimatingProperty);

        internal static readonly BindableProperty CancelAnimationProperty = BindableProperty.CreateAttached("CancelAnimation", typeof(bool), typeof(View), default(bool));

        internal static void SetCancelAnimation(BindableObject b, bool value) => b.SetValue(CancelAnimationProperty, value);

        internal static bool GetCancelAnimation(BindableObject b) => (bool)b.GetValue(CancelAnimationProperty);

        internal static readonly BindableProperty OriginalBackgroundColorProperty = BindableProperty.CreateAttached("OriginalBackgroundColor", typeof(Color), typeof(View), default(Color));

        internal static void SetOriginalBackgroundColor(BindableObject b, Color value) => b.SetValue(OriginalBackgroundColorProperty, value);

        internal static Color GetOriginalBackgroundColor(BindableObject b) => (Color)b.GetValue(OriginalBackgroundColorProperty);

        internal static readonly BindableProperty UseDynamicTextColorProperty = BindableProperty.CreateAttached("UseDynamicTextColor", typeof(bool), typeof(View), default(bool));

        internal static void SetUseDynamicTextColor(BindableObject b, bool value) => b.SetValue(UseDynamicTextColorProperty, value);

        internal static bool GetUseDynamicTextColor(BindableObject b) => (bool)b.GetValue(UseDynamicTextColorProperty);

        internal static readonly BindableProperty UseDynamicBackgroundColorProperty = BindableProperty.CreateAttached("UseDynamicBackground", typeof(bool), typeof(View), default(bool));

        internal static bool GetUseDynamicBackgroundColor(BindableObject b) => (bool)b.GetValue(UseDynamicBackgroundColorProperty);

        internal static void SetUseDynamicBackgroundColor(BindableObject b, bool value) => b.SetValue(UseDynamicBackgroundColorProperty, value);

        internal static readonly BindableProperty OriginalTextColorProperty = BindableProperty.CreateAttached("OriginalTextColor", typeof(Color), typeof(View), default(Color));

        internal static void SetOriginalTextColor(BindableObject b, Color value) => b.SetValue(OriginalTextColorProperty, value);

        internal static Color GetOriginalTextColor(BindableObject b) => (Color)b.GetValue(OriginalTextColorProperty);

        internal static readonly BindableProperty BaseAnimationProperty = BindableProperty.CreateAttached("BaseAnimation", typeof(BaseAnimation), typeof(View), null);

        internal static void SetBaseAnimation(BindableObject b, BaseAnimation value) => b.SetValue(BaseAnimationProperty, value);

        internal static BaseAnimation GetBaseAnimation(BindableObject b) => (BaseAnimation)b.GetValue(BaseAnimationProperty);

        #endregion Internal Properties

        #region Operations

        private static void OnIsBusyChanged(BindableObject bindable, bool newValue)
        {
            if (bindable.GetType().IsSubclassOf(typeof(View)))
            {
                HandleIsBusyChanged(bindable, newValue);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        static void HandleIsBusyChanged(BindableObject bindable, bool isBusyNewValue)         {             if (!(bindable is View))
                return;

            var view = (View)bindable;             if (isBusyNewValue)
            {
                if (GetHide(bindable))
                {
                    ((View)bindable).IsVisible = false;
                }
                else
                {
                    if (view is Layout layout && !GetIsParent(bindable))
                    {
                        SetLayoutChilds(layout);
                    }
                    else if (view is Label || view is Button)
                    {
                        SetTextColor(view);
                    }

                    SetBackgroundColor(view);

                    RunAnimation(view);
                }             }             else
            {
                if (GetHide(bindable))
                {
                    ((View)bindable).IsVisible = true;
                }
                else
                {
                    CancelAnimation(view);

                    RestoreBackgroundColor(view);

                    if (view is Layout layout && !GetIsParent(bindable))
                    {
                        RestoreLayoutChilds(layout);
                    }
                    else if (view is Label || view is Button)
                    {
                        RestoreTextColor(view);
                    }
                }
            }         }

        private static void SetLayoutChilds(Layout layout)
        {
            if (layout.Children != null && layout.Children.Count > 0)
            {
                layout.Children.ToList().ForEach(x => x.SetValue(VisualElement.OpacityProperty, 0));
            }
        }

        private static void RestoreLayoutChilds(Layout layout)
        {
            if (layout.Children != null && layout.Children.Count > 0)
            {
                layout.Children.ToList().ForEach(x => x.SetValue(VisualElement.OpacityProperty, 1));
            }
        }

        private static void SetBackgroundColor(View view)
        {
            var hasDynamic = GetUseDynamicBackgroundColor(view)
                || view.HasDynamicColorOnProperty(View.BackgroundColorProperty);

            var backgroundColor = GetBackgroundColor(view);
            if (backgroundColor != default(Color))
            {
                SetOriginalBackgroundColor(view, view.BackgroundColor);
                view.BackgroundColor = backgroundColor;
            }

            SetUseDynamicBackgroundColor(view, hasDynamic);
        }

        private static void RestoreBackgroundColor(View view)
        {
            var useDynamic = GetUseDynamicBackgroundColor(view);
            var backgroundColor = GetBackgroundColor(view);
            if (useDynamic)
            {
                view.ClearValue(View.BackgroundColorProperty);
            }
            else if (backgroundColor != default(Color))
            {
                view.BackgroundColor = GetOriginalBackgroundColor(view);
            }
        }

        private static void SetTextColor(View view)
        {
            var hasDynamic = GetUseDynamicTextColor(view);
            if (view is Label label)
            {
                hasDynamic = hasDynamic || label.HasDynamicColorOnProperty(Label.TextColorProperty);
                SetOriginalTextColor(label, label.TextColor);
                label.TextColor = Color.Transparent;
            }
            else if (view is Button button)
            {
                hasDynamic = hasDynamic || button.HasDynamicColorOnProperty(Button.TextColorProperty);
                SetOriginalTextColor(button, button.TextColor);
                button.TextColor = Color.Transparent;
            }

            SetUseDynamicTextColor(view, hasDynamic);
        }

        private static void RestoreTextColor(View view)
        {
            var useDynamic = GetUseDynamicTextColor(view);
            if (view is Label label)
            {
                if (useDynamic)
                {
                    var key = label.GetPropertyDynamicResourceKey(Label.TextColorProperty);
                    label.SetDynamicResource(Label.TextColorProperty, key);
                }
                else
                {
                    label.TextColor = GetOriginalTextColor(view);
                }
            }
            else if (view is Button button)
            {
                if (useDynamic)
                {
                    var key = button.GetPropertyDynamicResourceKey(Button.TextColorProperty);
                    button.SetDynamicResource(Button.TextColorProperty, key);
                }
                else
                {
                    button.TextColor = GetOriginalTextColor(view);
                }
            }
        }

        private static void RunAnimation(View view)
        {
            var animationType = GetAnimation(view);

            if (animationType == AnimationTypes.None || GetAnimating(view))
                return;

            SetCancelAnimation(view, false);

            BaseAnimation animation = null;

            switch (animationType)
            {
                case AnimationTypes.Beat:
                    animation = new BeatAnimation(GetAnimationInterval(view), GetAnimationParameter(view));
                    break;
                case AnimationTypes.Fade:
                    animation = new FadeAnimation(GetAnimationInterval(view), GetAnimationParameter(view));
                    break;
                case AnimationTypes.VerticalShake:
                    animation = new VerticalShakeAnimation(GetAnimationInterval(view), GetAnimationParameter(view));
                    break;
                case AnimationTypes.HorizontalShake:
                    animation = new HorizontalShakeAnimation(GetAnimationInterval(view), GetAnimationParameter(view));
                    break;
                default:
                    break;
            }

            if (animation != null)
            {
                SetBaseAnimation(view, animation);
                animation.Start(view);
            }
        }

        private static void CancelAnimation(View view)
        {
            if (GetAnimation(view) == AnimationTypes.None)
                return;

            SetCancelAnimation(view, true);

            var animation = GetBaseAnimation(view);
            animation.Stop(view);
        }

        #endregion Operations
    }
}
