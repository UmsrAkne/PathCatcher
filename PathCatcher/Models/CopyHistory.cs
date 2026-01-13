using System;

namespace PathCatcher.Models
{
    public class CopyHistory
    {
        public DateTime DateTime { get; set; } = DateTime.Now;

        public string FilePath { get; set; } = string.Empty;
    }
}