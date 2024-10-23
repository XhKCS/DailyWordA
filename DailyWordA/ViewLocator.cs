using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using DailyWordA.Library.ViewModels;

namespace DailyWordA;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().
            FullName!.Replace("ViewModel", "View", StringComparison.Ordinal)
            .Replace("DailyWordA.Library", "DailyWordA");
        var type = Type.GetType(name); //获取到该ViewModel对应的View类

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}