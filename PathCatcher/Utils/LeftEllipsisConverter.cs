using System;
using System.Globalization;
using System.Windows.Data;

namespace PathCatcher.Utils
{
    /// <summary>
    /// Truncates a string from the left and prefixes it with an ellipsis ("...") to fit a target length.
    /// Use ConverterParameter to specify the target visible length (default: 60).
    /// </summary>
    public class LeftEllipsisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value?.ToString() ?? string.Empty;

            // Try to parse desired length from parameter; fall back to default if invalid
            var targetLength = 60;
            if (parameter != null)
            {
                if (parameter is int i)
                {
                    targetLength = i;
                }
                else if (int.TryParse(parameter.ToString(), out var parsed))
                {
                    targetLength = parsed;
                }
            }

            if (targetLength <= 0)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(input) || input.Length <= targetLength)
            {
                return input;
            }

            // If the requested length is very small, just return dots up to the length
            if (targetLength <= 3)
            {
                return new string('.', targetLength);
            }

            var tailLength = targetLength - 3; // account for "..."

            // Keep the last tailLength characters
            return string.Concat("...", input.AsSpan(input.Length - tailLength, tailLength));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-way conversion only
            return Binding.DoNothing;
        }
    }
}