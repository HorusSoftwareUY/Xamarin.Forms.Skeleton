using System;
using System.Linq;
#if NET6_0_OR_GREATER
using Maui.Skeleton.Animations;
using Maui.Skeleton.Extensions;
#else
using Xamarin.Forms.Skeleton.Animations;
using Xamarin.Forms.Skeleton.Extensions;
#endif


#if NET6_0_OR_GREATER
namespace Maui.Skeleton
#else
namespace Xamarin.Forms.Skeleton
#endif
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

        public static readonly BindableProperty AnimationProperty = BindableProperty.CreateAttached("Animation", typeof(BaseAnimation), typeof(View), null);

        public static void SetAnimation(BindableObject b, BaseAnimation value) => b.SetValue(AnimationProperty, value);

        public static BaseAnimation GetAnimation(BindableObject b) => (BaseAnimation)b.GetValue(AnimationProperty);

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

        internal static readonly BindableProperty OriginalOpacityProperty = BindableProperty.CreateAttached("OriginalOpacity", typeof(double), typeof(View), 1d);

        internal static void SetOriginalOpacity(BindableObject b, double value) => b.SetValue(OriginalOpacityProperty, value);

        internal static double GetOriginalOpacity(BindableObject b) => (double)b.GetValue(OriginalOpacityProperty);

        internal static readonly BindableProperty OriginalTextColorProperty = BindableProperty.CreateAttached("OriginalTextColor", typeof(Color), typeof(View), default(Color));

        internal static void SetOriginalTextColor(BindableObject b, Color value) => b.SetValue(OriginalTextColorProperty, value);

        internal static Color GetOriginalTextColor(BindableObject b) => (Color)b.GetValue(OriginalTextColorProperty);

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

        static void HandleIsBusyChanged(BindableObject bindable, bool isBusyNewValue)
        {
            if (!(bindable is View))
                return;

            var view = (View)bindable;
            if (isBusyNewValue)
            {
                if (GetHide(bindable))
                {
                    ((View)bindable).IsVisible = false;
                }
                else
                {
                    if (view is Label || view is Button)
                    {
                        SetTextColor(view);
                    }
                    else if (!GetIsParent(bindable))
                    {
                        HideContent(view);
                    }

                    SetBackgroundColor(view);

                    RunAnimation(view);
                }
            }
            else
            {
                if (GetHide(bindable))
                {
                    ((View)bindable).IsVisible = true;
                }
                else
                {
                    CancelAnimation(view);

                    RestoreBackgroundColor(view);

                    if (view is Label || view is Button)
                    {
                        RestoreTextColor(view);
                    }
                    else if (!GetIsParent(bindable))
                    {
                        RestoreContent(view);
                    }
                }
            }
        }

        /// <summary>
        /// Fades out whatever the container is showing, so the placeholder colour is what stays
        /// visible, remembering each child's own opacity first. A view that was deliberately
        /// translucent has to come back translucent, which is why this mirrors how the background
        /// and text colours are saved rather than restoring a hard-coded 1.
        /// </summary>
        private static void HideContent(View view)
        {
            ForEachContentChild(view, child =>
            {
                SetOriginalOpacity(child, child.Opacity);
                child.SetValue(VisualElement.OpacityProperty, 0d);
            });
        }

        private static void RestoreContent(View view)
        {
            ForEachContentChild(view, child =>
                child.SetValue(VisualElement.OpacityProperty, GetOriginalOpacity(child)));
        }

        /// <summary>
        /// Walks whatever a container is showing. A Layout arranges several children, while a
        /// Border, Frame or ContentView holds one. The second group needs its own branch because in
        /// MAUI those do not derive from Layout, and without it their content stayed fully visible
        /// on top of the placeholder.
        /// </summary>
        private static void ForEachContentChild(View view, Action<View> action)
        {
            if (view is Layout layout)
            {
                if (layout.Children == null || layout.Children.Count == 0)
                    return;

                foreach (var child in layout.Children.ToList())
                {
                    // Not every child of a layout is necessarily a View.
                    if (child is View childView)
                        action(childView);
                }

                return;
            }

#if NET6_0_OR_GREATER
            if (view is IContentView contentView && contentView.PresentedContent is View content)
            {
                action(content);
            }
#endif
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
                // A view that never declared a colour saved null here, and writing null back leaves
                // the placeholder on screen: neither that nor ClearValue repaints, measured on
                // device. Assigning does, so the fallback is transparent, which is what such a view
                // looked like to begin with.
                var original = GetOriginalBackgroundColor(view);
#if NET6_0_OR_GREATER
                view.BackgroundColor = original ?? Colors.Transparent;
#else
                view.BackgroundColor = original == default(Color) ? Color.Transparent : original;
#endif
            }
        }

        private static void SetTextColor(View view)
        {
            var hasDynamic = GetUseDynamicTextColor(view);
            // Only a control that already has a colour of its own can have one put back. The
            // platform's default text colour is not something this can read, and writing null or
            // clearing does not repaint, so hiding such a control would be permanent. It is left
            // alone instead: put a Label inside a container that carries the placeholder colour, the
            // way the samples do, and the container covers it. See issue #50.
            if (GetTextColorOf(view) == default(Color))
                return;

            if (view is Label label)
            {
                hasDynamic = hasDynamic || label.HasDynamicColorOnProperty(Label.TextColorProperty);
                SetOriginalTextColor(label, label.TextColor);
#if NET6_0_OR_GREATER
                label.TextColor = Colors.Transparent;
#else
                label.TextColor = Color.Transparent;
#endif
            }
            else if (view is Button button)
            {
                hasDynamic = hasDynamic || button.HasDynamicColorOnProperty(Button.TextColorProperty);
                SetOriginalTextColor(button, button.TextColor);
#if NET6_0_OR_GREATER
                button.TextColor = Colors.Transparent;
#else
                button.TextColor = Color.Transparent;
#endif
            }

            SetUseDynamicTextColor(view, hasDynamic);
        }

        /// <summary>
        /// The colour a text control currently carries, or null when it never declared one and is
        /// drawing with whatever the platform decided.
        /// </summary>
        private static Color GetTextColorOf(View view)
        {
            if (view is Label label)
                return label.TextColor;

            if (view is Button button)
                return button.TextColor;

            return default(Color);
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
            var animation = GetAnimation(view);

            if (animation == null || GetAnimating(view))
                return;

            SetCancelAnimation(view, false);

            if (animation != null)
            {
                animation.Start(view);
            }
        }

        private static void CancelAnimation(View view)
        {
            var animation = GetAnimation(view);

            if (animation == null)
                return;

            SetCancelAnimation(view, true);

            animation.Stop(view);
        }

        #endregion Operations
    }
}