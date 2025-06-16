using Avalonia.Data.Converters;
using System.Globalization;
using System;

namespace Inv.ViewModels.Converters;

public class NullableDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
            return date.ToString("dd.MM.yyyy");
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DateTime.TryParse(value?.ToString(), out var result) ? result : (DateTime?)null;
    }
}
