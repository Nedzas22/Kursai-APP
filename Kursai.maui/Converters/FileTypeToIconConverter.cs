using System.Globalization;

namespace Kursai.maui.Converters
{
    public class FileTypeToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return "FILE";

            var fileType = value.ToString()?.ToLowerInvariant() ?? "";

            // Video files
            if (fileType.Contains("video") || fileType.Contains("mp4") || 
                fileType.Contains("avi") || fileType.Contains("mov") || 
                fileType.Contains("wmv"))
            {
                return "VIDEO";
            }

            // PDF files
            if (fileType.Contains("pdf"))
            {
                return "PDF";
            }

            // Document files
            if (fileType.Contains("doc") || fileType.Contains("word"))
            {
                return "DOC";
            }

            // Default
            return "FILE";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
