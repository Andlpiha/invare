using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Inv.ViewModels.Converters
{
    internal class ArrowIconConverter : IMultiValueConverter
    {
        public static readonly ArrowIconConverter Instance = new();

        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if(values?.Count != 2 || !targetType.IsAssignableTo(typeof(string)))
                //  Неверное использование
                return new BindingNotification(new InvalidCastException(),
                                                    BindingErrorType.Error);
            if (!values.All(x => x is bool or UnsetValueType or null))
                return new BindingNotification(new InvalidCastException(),
                                                    BindingErrorType.Error);

            // Если значения не выставлены, ничего не делаем
            if (values[0] is not bool isEnabled ||
                values[1] is not bool IsChecked)
                return BindingOperations.DoNothing;

            if (!isEnabled)
                return "/Assets/other-icons/edit-note.svg";
            return IsChecked ? "/Assets/other-icons/minus.svg" : "/Assets/other-icons/plus.svg";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
