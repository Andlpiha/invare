using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Inv.ViewModels.Converters
{
    public class IconIndexConverter : IValueConverter
    {
        public static readonly IconIndexConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter,
                                                                CultureInfo culture)
        {
            if (value is int)
            {
                switch ((int)value)
                {
                    case Global.ComplectIcon:
                        return "/Assets/toolbar-icons/desktop-tower.svg";
                    default:
                        return "/Assets/toolbar-icons/expansion-card-variant.svg";
                }
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(),
                                                    BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType,
                                    object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
