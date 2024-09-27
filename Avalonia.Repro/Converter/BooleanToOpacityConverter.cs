using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Avalonia.Repro.Converter
{
    public class BooleanToOpacityConverter : AvaloniaObject, IValueConverter
    {
        public double Active
        {
            get { return GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Active.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<double> ActiveProperty =
            AvaloniaProperty.Register<BooleanToOpacityConverter, double>(nameof(Active), 1);

        public double Default
        {
            get { return GetValue(DefaultProperty); }
            set { SetValue(DefaultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Default.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<double> DefaultProperty =
            AvaloniaProperty.Register<BooleanToOpacityConverter, double>(nameof(Active), 0.7);

        public BooleanToOpacityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Equals(value, true) ? Active : Default;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
