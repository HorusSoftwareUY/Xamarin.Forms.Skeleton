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

        internal static readonly BindableProperty TextColorFromPlatformProperty = BindableProperty.CreateAttached("TextColorFromPlatform", typeof(bool), typeof(View), default(bool));

        internal static void SetTextColorFromPlatform(BindableObject b, bool value) => b.SetValue(TextColorFromPlatformProperty, value);

        internal static bool GetTextColorFromPlatform(BindableObject b) => (bool)b.GetValue(TextColorFromPlatformProperty);

        internal static readonly BindableProperty TextColorDeferredProperty = BindableProperty.CreateAttached("TextColorDeferred", typeof(bool), typeof(View), default(bool));

        internal static void SetTextColorDeferred(BindableObject b, bool value) => b.SetValue(TextColorDeferredProperty, value);

        internal static bool GetTextColorDeferred(BindableObject b) => (bool)b.GetValue(TextColorDeferredProperty);

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

            // What to put back when loading ends. A control that declared a colour hands it over
            // directly; one drawing with the platform's has it read off the native control. When
            // neither works there is nothing that could restore it, so it is left readable rather
            // than hidden for good. See issue #50.
            var toRestore = GetTextColorOf(view);
            var fromPlatform = toRestore == default(Color);
            if (fromPlatform)
                toRestore = TryReadPlatformTextColor(view);

            if (toRestore == default(Color))
            {
                // Nothing readable yet. On first load that is normal rather than final: IsBusy is
                // typically already true before the native control exists, so there is no colour to
                // read at this point. Wait for it and hide then, instead of giving up and letting
                // the text sit on top of the placeholder.
                DeferTextColorUntilHandler(view);
                return;
            }

            SetTextColorFromPlatform(view, fromPlatform);

            if (view is Label label)
            {
                hasDynamic = hasDynamic || label.HasDynamicColorOnProperty(Label.TextColorProperty);
                SetOriginalTextColor(label, toRestore);
#if NET6_0_OR_GREATER
                label.TextColor = Colors.Transparent;
#else
                label.TextColor = Color.Transparent;
#endif
            }
            else if (view is Button button)
            {
                hasDynamic = hasDynamic || button.HasDynamicColorOnProperty(Button.TextColorProperty);
                SetOriginalTextColor(button, toRestore);
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

        /// <summary>
        /// Turns a boxed native number into a double.
        /// <para>
        /// iOS hands colour channels back as <c>NFloat</c>, which does not implement
        /// <c>IConvertible</c>, so <c>Convert.ToDouble</c> throws on it. It does carry a
        /// <c>Value</c> of type double, which is what this reaches for first.
        /// </para>
        /// </summary>
        private static double ToDouble(object value)
        {
            if (value is double asDouble)
                return asDouble;

            if (value is float asFloat)
                return asFloat;

            var valueProperty = value != null ? value.GetType().GetProperty("Value") : null;
            if (valueProperty != null && valueProperty.PropertyType == typeof(double))
                return (double)valueProperty.GetValue(value);

            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Reads a property off a native object, or null when the type does not have it.
        /// </summary>
        private static object ReadProperty(object instance, Type type, string name)
        {
            var property = type.GetProperty(name);
            return property != null ? property.GetValue(instance) : null;
        }

        /// <summary>
        /// Hands a platform colour back to the platform after it has been repainted.
        /// <para>
        /// The colour read off the native control is a snapshot of one moment. Assigning it is what
        /// repaints — writing null does nothing — but leaving it assigned would pin a control that
        /// used to follow the platform to that snapshot, so it would keep the dark theme's text
        /// colour after the user switches to the light one. Clearing straight after drops the local
        /// value without repainting, so the pixels stay correct now and the platform is free to
        /// resolve the colour itself again later.
        /// </para>
        /// </summary>
        private static void ReleasePlatformTextColor(View view, BindableProperty property)
        {
            if (!GetTextColorFromPlatform(view))
                return;

            SetTextColorFromPlatform(view, false);
            view.ClearValue(property);
        }

        /// <summary>
        /// Runs the hiding again once the native control is there to be read. Does nothing when a
        /// handler already exists, which makes this terminate: the second attempt cannot re-arm it.
        /// </summary>
        private static void DeferTextColorUntilHandler(View view)
        {
#if NET6_0_OR_GREATER
            // Only ever one armed at a time. IsBusy can go true, false and true again before the
            // native control exists, and two callbacks would both run when it arrives: the first
            // hides the text, the second then reads that transparency and saves it as the colour to
            // restore, leaving the control blank for good.
            if (view.Handler != null || GetTextColorDeferred(view))
                return;

            SetTextColorDeferred(view, true);

            EventHandler onHandlerChanged = null;
            onHandlerChanged = (sender, args) =>
            {
                view.HandlerChanged -= onHandlerChanged;
                SetTextColorDeferred(view, false);

                if (GetIsBusy(view))
                    SetTextColor(view);
            };

            view.HandlerChanged += onHandlerChanged;
#endif
        }

        /// <summary>
        /// The colour the platform is actually drawing this control's text with, or null when it
        /// cannot be read.
        /// <para>
        /// Only needed for a control that declared no colour of its own: there is nothing to save,
        /// and writing null back on the way out does not repaint, which is how #50 left text hidden
        /// for good. The value is read off the native control instead of guessed. Guessing was
        /// measured and is wrong: on a dark-themed device the platform draws #BCBCBC, not white.
        /// </para>
        /// <para>
        /// This library targets plain <c>net8.0</c> and up with no platform-specific code, so
        /// <c>Handler.PlatformView</c> is only an <c>object</c> here and the native type cannot be
        /// named. Reflection is what is left. Android and Windows both expose the resolved colour;
        /// where nothing matches, this returns null and the caller leaves the control alone rather
        /// than hiding text it would not be able to bring back.
        /// </para>
        /// </summary>
        private static Color TryReadPlatformTextColor(View view)
        {
#if NET6_0_OR_GREATER
            var platformView = view.Handler != null ? view.Handler.PlatformView : null;
            if (platformView == null)
                return null;

            var type = platformView.GetType();

            try
            {
                // Android: TextView.CurrentTextColor, and MaterialButton derives from it. The value
                // is a packed ARGB int.
                var currentTextColor = type.GetProperty("CurrentTextColor");
                if (currentTextColor != null && currentTextColor.PropertyType == typeof(int))
                {
                    var argb = (int)currentTextColor.GetValue(platformView);
                    return Color.FromRgba(
                        ((argb >> 16) & 0xFF) / 255.0,
                        ((argb >> 8) & 0xFF) / 255.0,
                        (argb & 0xFF) / 255.0,
                        ((argb >> 24) & 0xFF) / 255.0);
                }

                // iOS and Mac Catalyst: UILabel.TextColor, or UIButton.CurrentTitleColor. Both hand
                // back a UIColor, whose channels only come out through GetRGBA's out parameters,
                // which is what the boxed argument array is for.
                var uiColor = ReadProperty(platformView, type, "TextColor")
                    ?? ReadProperty(platformView, type, "CurrentTitleColor");
                if (uiColor != null)
                {
                    var getRgba = uiColor.GetType().GetMethod("GetRGBA");
                    if (getRgba != null && getRgba.GetParameters().Length == 4)
                    {
                        var channels = new object[4];
                        getRgba.Invoke(uiColor, channels);
                        return Color.FromRgba(
                            ToDouble(channels[0]),
                            ToDouble(channels[1]),
                            ToDouble(channels[2]),
                            ToDouble(channels[3]));
                    }
                }

                // Windows: TextBlock.Foreground is a SolidColorBrush carrying byte channels.
                var brush = ReadProperty(platformView, type, "Foreground");
                var native = brush != null ? ReadProperty(brush, brush.GetType(), "Color") : null;
                if (native != null)
                {
                    var nativeType = native.GetType();
                    if (nativeType.GetProperty("R") != null && nativeType.GetProperty("A") != null)
                    {
                        return Color.FromRgba(
                            ToDouble(ReadProperty(native, nativeType, "R")) / 255.0,
                            ToDouble(ReadProperty(native, nativeType, "G")) / 255.0,
                            ToDouble(ReadProperty(native, nativeType, "B")) / 255.0,
                            ToDouble(ReadProperty(native, nativeType, "A")) / 255.0);
                    }
                }
            }
            catch
            {
                // Reading a native property is best effort. Anything going wrong here means the
                // colour is unknown, which the caller already handles by leaving the control alone.
                // It must never take the app down.
            }
#endif
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
                    ReleasePlatformTextColor(label, Label.TextColorProperty);
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
                    ReleasePlatformTextColor(button, Button.TextColorProperty);
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