using System.IO;
using PathCatcher.Models;

namespace PathCatcher.Utils
{
    public static class ContentTypeDetector
    {
        public static ContentType DetectContentType(string filePath)
        {
            var ext = Path.GetExtension(filePath)?.ToLowerInvariant();

            return ext switch
            {
                ".png" or ".jpg" or ".jpeg" or ".webp" or ".bmp" or ".gif"
                    => ContentType.Image,

                ".txt" or ".md" or ".json" or ".yaml" or ".yml" or ".csv" or ".log"
                    => ContentType.Text,

                _ => ContentType.Other,
            };
        }
    }
}