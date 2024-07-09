using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ContactKeeper.UI.ValueConverters;

/// <summary>
/// Converts a string to a mailto hyperlink.
/// </summary>
internal class MailToHyperLinkConverter : IValueConverter
{
    /// <summary>
    /// Converts a string to a mailto hyperlink.
    /// </summary>
    /// <param name="value">The email address.</param>
    /// <param name="targetType">Not used.</param>
    /// <param name="parameter">Not used.</param>
    /// <param name="culture">Not used.</param>
    /// <returns>A mailto hyperlink.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string email)
        {
            return $"mailto:{email}";
        }

        return DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
