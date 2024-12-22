using System;
using System.Globalization;
using Avalonia.Data.Converters;
using DailyWordA.Library.Models;

namespace DailyWordA.Converters;

public class MottoToStringConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, 
        CultureInfo culture)
    {
        return value is DailyMotto dailyMotto
            ? $"{dailyMotto.Translation}"
            : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        throw new InvalidOperationException();
}