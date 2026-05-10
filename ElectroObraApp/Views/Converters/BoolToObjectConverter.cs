using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Linq;

namespace ElectroObraApp.Views.Converters;

public class BoolToObjectConverter : IValueConverter
{
    public object? TrueValue { get; set; }
    public object? FalseValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? TrueValue : FalseValue;
        }
        
        // Soporte para formato string "TrueValue|FalseValue" vía parámetro si no se setean propiedades
        if (parameter is string p && p.Contains('|'))
        {
            var parts = p.Split('|');
            bool val = value is bool b2 && b2;
            return val ? parts[0] : parts[1];
        }

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
