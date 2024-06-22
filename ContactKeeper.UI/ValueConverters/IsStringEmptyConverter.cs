using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace ContactKeeper.UI.ValueConverters;

/// <summary>
/// Converts a string to a boolean value indicating whether the string is empty.
/// </summary>
public class IsStringEmptyConverter : IValueConverter
{
    private const string InvertParameterString = "invert";

    /// <summary>
    /// Converts a string to a boolean value indicating whether the string is empty.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="targetType">Unused.</param>
    /// <param name="parameter">The converter parameter to use.
    /// If the parameter is "invert", the result is inverted.</param>
    /// <param name="culture">Unused.</param>
    /// <returns>True if the string is empty; otherwise, false.
    /// Unless the parameter is "invert", in which case the result is inverted.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is not string str)
            return DependencyProperty.UnsetValue;

        var isStringEmpty = string.IsNullOrEmpty(str);

        if (parameter is string invert && string.Equals(invert, InvertParameterString, StringComparison.OrdinalIgnoreCase))
        {
            isStringEmpty = !isStringEmpty;
        }

        return isStringEmpty;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
