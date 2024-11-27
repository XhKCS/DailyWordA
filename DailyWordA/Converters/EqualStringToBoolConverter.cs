using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DailyWordA.Converters;

public class EqualStringToBoolConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        bool res = value is string stringValue 
               && parameter is string stringParam && stringValue == stringParam;
        return res;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new InvalidOperationException();
    }
}