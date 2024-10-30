using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DailyWordA.Converters;

public class NegativeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        value is bool b ? !b : null;

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        throw new InvalidOperationException();
}