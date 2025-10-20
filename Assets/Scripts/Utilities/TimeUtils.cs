using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utilities
{
    public class TimeUtils
    {
        public static string FormatTime(int seconds, string format = "HH:mm:ss")
        {
            var hours = seconds / 3600;
            var minutes = (seconds % 3600) / 60;
            var secs = seconds % 60;
            var totalMinutes = seconds / 60;

            var replacements = new Dictionary<string, string>
            {
                { "HH", Pad(hours) },
                { "MM", Pad(totalMinutes) },
                { "mm", Pad(minutes) },
                { "ss", Pad(secs) }
            };

            return Regex.Replace(format, "HH|MM|mm|ss", match => replacements[match.Value]);

            string Pad(int num) => num.ToString("D2");
        }
    }
}