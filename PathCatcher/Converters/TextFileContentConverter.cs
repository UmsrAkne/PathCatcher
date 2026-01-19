using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;

namespace PathCatcher.Converters
{
    /// <summary>
    ///     Converts a file path to the UTF-8 text content of the file.
    ///     Returns an empty string when the path is invalid or unreadable.
    /// </summary>
    public class TextFileContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string path || string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            if (!File.Exists(path))
            {
                return string.Empty;
            }

            try
            {
                return File.ReadAllText(path, Encoding.UTF8);
            }
            catch (IOException)
            {
                return string.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}