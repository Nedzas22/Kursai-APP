using System.Globalization;

namespace Kursai.maui.Converters
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double rating)
                return "-----"; // No rating

            // Round to nearest 0.5
            double roundedRating = Math.Round(rating * 2) / 2;
            int fullStars = (int)Math.Floor(roundedRating);
            bool hasHalfStar = (roundedRating - fullStars) >= 0.5;
            int emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

            string stars = "";
            
            // Add full stars - using filled star
            for (int i = 0; i < fullStars; i++)
            {
                stars += "?";
            }
            
            // Add half star if needed
            if (hasHalfStar)
            {
                stars += "?";
            }
            
            // Add empty stars - using outlined star (or we can skip them)
            // For compact display, let's just show filled stars
            
            // If no stars, show placeholder
            if (string.IsNullOrEmpty(stars))
            {
                return "-----";
            }

            return stars;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
