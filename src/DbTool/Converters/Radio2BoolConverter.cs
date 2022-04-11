using System;
using System.Globalization;
using System.Windows.Data;

namespace DbTool.Converters;

public class Radio2BoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null && (value.ToString()?.Equals(parameter) ?? false);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null && value.Equals(true) ? parameter : Binding.DoNothing;
    }
}