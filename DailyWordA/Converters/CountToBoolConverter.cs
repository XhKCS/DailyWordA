using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DailyWordA.Converters;

public class CountToBoolConverter : IValueConverter {
    // parameter是目标阈值
    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        value is int count && parameter is string conditionString &&
        int.TryParse(conditionString, out var condition)
            ? count > condition
            : null;

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        throw new InvalidOperationException();
}