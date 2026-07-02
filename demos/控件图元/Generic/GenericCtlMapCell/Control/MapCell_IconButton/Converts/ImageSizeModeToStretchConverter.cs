using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    public class ImageSizeModeToStretchConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                ImageSizeMode.Fill => Stretch.Fill,
                ImageSizeMode.Uniform => Stretch.Uniform,
                ImageSizeMode.UniformToFill => Stretch.UniformToFill,
                ImageSizeMode.None => Stretch.None,
                _ => Stretch.None
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Stretch.Fill => ImageSizeMode.Fill,
                Stretch.Uniform => ImageSizeMode.Uniform,
                Stretch.UniformToFill => ImageSizeMode.UniformToFill,
                Stretch.None => ImageSizeMode.None,
                _ => ImageSizeMode.None
            };
        }
    }
}
