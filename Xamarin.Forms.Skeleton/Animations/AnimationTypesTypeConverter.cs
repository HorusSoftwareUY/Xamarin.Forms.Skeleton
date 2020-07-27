using System;

namespace Xamarin.Forms.Skeleton.Animations
{
    [Xaml.TypeConversion(typeof(BaseAnimation))]
    public class AnimationTypesTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            var type = (AnimationTypes)Enum.Parse(typeof(AnimationTypes), value);
            return new DefaultAnimationExtension() { Source = type }.ProvideValue(null);
        }
    }
}
