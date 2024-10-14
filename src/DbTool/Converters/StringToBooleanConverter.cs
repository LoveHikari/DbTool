using System.Globalization;
using System.Windows.Data;

namespace DbTool.Converters;

public class StringToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        return value.ToString().Equals(parameter.ToString());
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null && (bool)value)
            return parameter ?? "";

        return Binding.DoNothing;
    }
}
