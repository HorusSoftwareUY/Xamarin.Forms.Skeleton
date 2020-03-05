using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Skeleton.Extensions
{
    internal static class ViewExtensions
    {
        #region Internal Methods

        /// <summary>
        /// Check if property has dynamic color resource associated
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <param name="view">View object</param>
        /// <param name="property">Bindable property to check</param>
        /// <returns>True if any associated Setter found</returns>
        internal static bool HasDynamicColorOnProperty<TView>(this TView view, BindableProperty property) where TView : View
        {
            var resourceValue = view?.GetPropertyDynamicResourceValue(property);
            if (resourceValue == null) return false;

            var currentValue = GetValueFromPropertyName(view, property.PropertyName);
            return currentValue != null && (Color)currentValue == (Color)resourceValue;
        }

        /// <summary>
        /// Get value from dynamic resource associated to property
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <param name="view">View object</param>
        /// <param name="property">Bindable property to check</param>
        /// <returns>Associated resource value</returns>
        internal static object GetPropertyDynamicResourceValue<TView>(this TView view, BindableProperty property) where TView : View
        {
            var resourceKey = view.GetPropertyDynamicResourceKey(property);

            if (string.IsNullOrWhiteSpace(resourceKey))
                return null;

            var currentResources = Application.Current.Resources;
            if (!currentResources.TryGetValue(resourceKey, out var resourceValue))
                return null;

            return resourceValue;
        }

        /// <summary>
        /// Get key from dynamic resource associated to property
        /// </summary>
        /// <typeparam name="TView">View type</typeparam>
        /// <param name="view">View object</param>
        /// <param name="property">Bindable property to check</param>
        /// <returns>Associated resource key</returns>
        internal static string GetPropertyDynamicResourceKey<TView>(this TView view, BindableProperty propertyToCheck) where TView : View
        {
            var elementStyle = view?.Style;
            var styleSetters = GetAllSetters(elementStyle);
            var setter = styleSetters?.FirstOrDefault(
                s => s.Property.PropertyName == propertyToCheck.PropertyName);

            if (setter?.Value is DynamicResource dynamicResource)
                return dynamicResource.Key;

            return null;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<Setter> GetAllSetters(Style style)
        {
            if (style == null)
                return Enumerable.Empty<Setter>();

            return style.Setters.Concat(GetAllSetters(style.BasedOn));
        }

        private static object GetValueFromPropertyName(object view, string propertyToCheck) =>
            view.GetType().GetProperty(propertyToCheck).GetValue(view);

        #endregion
    }
}
