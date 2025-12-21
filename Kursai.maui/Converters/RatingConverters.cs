using System.Globalization;

namespace Kursai.maui.Converters
{
    public class IsGreaterThanZeroConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue > 0;
            
            if (value is double doubleValue)
                return doubleValue > 0;
                
            if (value is decimal decimalValue)
                return decimalValue > 0;

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RatingButtonColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int selectedRating && parameter is string starPositionStr)
            {
                if (int.TryParse(starPositionStr, out int starPosition))
                {
                    // Return gold color if this star is selected or lower, gray otherwise
                    return selectedRating >= starPosition ? Color.FromArgb("#FFD700") : Color.FromArgb("#808080");
                }
            }

            return Color.FromArgb("#808080"); // Default to gray
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
