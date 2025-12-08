using System.Globalization;
using Microsoft.Maui.Controls;

namespace Kursai.maui.Converters
{
    public class IsNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return false;
            
            if (value is string stringValue)
                return !string.IsNullOrEmpty(stringValue);
            
            if (value is ImageSource)
                return true;
            
            // For other object types, check if not null
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException("IsNotNullOrEmptyConverter does not support two-way binding");
        }
    }
}