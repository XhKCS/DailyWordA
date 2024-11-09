using System;
using System.Globalization;
using Avalonia.Data.Converters;
using DailyWordA.Library.Models;

namespace DailyWordA.Converters;

public class WordToStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        value is WordObject wordObject
            ? $" {wordObject.Accent}   {wordObject.CnMeaning}"
            : null;

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        throw new InvalidOperationException();
}