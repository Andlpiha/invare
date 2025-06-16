using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Media.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Inv.ViewModels.Converters
{
    internal class DateProfConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count != 2 || !targetType.IsAssignableFrom(typeof(ImmutableSolidColorBrush)))
                return Avalonia.AvaloniaProperty.UnsetValue;

            DateTime current_date = new DateTime(2023, 11, 23);
            DateTime prof_date;
            int prof_interval;

            if (values[0] is DateTime && values[1] is int)
            {
                prof_date = (DateTime)values[0]!;
                prof_interval = (int)values[1]!;
            }
            else if (values[1] is DateTime && values[0] is int)
            {
                prof_date = (DateTime)values[1]!;
                prof_interval = (int)values[0]!;
            }
            else
                return Avalonia.AvaloniaProperty.UnsetValue;
            if(prof_interval == 0)
                return Avalonia.AvaloniaProperty.UnsetValue;

            if (prof_date.AddDays(prof_interval).CompareTo(current_date) <= 0)
                return new ImmutableSolidColorBrush(Avalonia.Media.Color.FromArgb(150, 255 , 0, 0));
            else if(prof_date.AddDays(prof_interval - 7).CompareTo(current_date) <= 0)
                return new ImmutableSolidColorBrush(Avalonia.Media.Color.FromArgb(150, 255, 255, 0));
            else
                return Avalonia.AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
