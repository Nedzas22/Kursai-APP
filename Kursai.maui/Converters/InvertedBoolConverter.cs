using System.Globalization;
using Microsoft.Maui.Controls;

namespace Kursai.maui.Converters
{
    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return true; // Default to true when null
            
            if (value is bool boolValue)
                return !boolValue;
            
            return true; // Default to true for non-bool values
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return false; // Default to false when null
            
            if (value is bool boolValue)
                return !boolValue;
            
            return false; // Default to false for non-bool values
        }
    }
}