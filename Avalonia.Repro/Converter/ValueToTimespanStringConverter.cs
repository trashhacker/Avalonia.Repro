using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.Repro.Converter
{
    public class ValueToTimespanStringConverter : IMultiValueConverter
    {
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            string xxx = string.Join(":", values.Select(x => $"{x:00}"));
            return xxx;
        }
    }
}
