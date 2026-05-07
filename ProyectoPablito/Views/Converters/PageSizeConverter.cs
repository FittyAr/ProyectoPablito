using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ProyectoPablito.Views.Converters;

public class PageSizeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int pageSize)
        {
            return pageSize == 0 ? "Todos" : pageSize.ToString();
        }
        return value ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
