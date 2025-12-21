using System.Globalization;

namespace Kursai.maui.Converters
{
    /// <summary>
    /// Alternative converter that returns a text representation of the rating
    /// Use this if star emojis don't render properly
    /// </summary>
    public class RatingToTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double rating || rating == 0)
                return "No rating";

            // Return format like "4.5 ?"
            return $"{rating:F1} ?";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns a simple star count like "????" without empty stars
    /// </summary>
    public class RatingToSimpleStarsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double rating || rating == 0)
                return "";

            int starCount = (int)Math.Round(rating);
            return new string('?', starCount);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
