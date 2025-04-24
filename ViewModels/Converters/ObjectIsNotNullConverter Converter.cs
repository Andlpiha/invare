using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inv.ViewModels.Converters
{
    public class ObjectIsNotNullConverter : IValueConverter
    {
        public static readonly ObjectIsNotNullConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
