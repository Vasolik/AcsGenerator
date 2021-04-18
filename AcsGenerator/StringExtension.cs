using System.Collections.Generic;
using System.Linq;

namespace Vipl.AcsGenerator
{
    public static class StringExtension
    {
        public static string Intend(this string value, int count, bool first = false)
        {
            var indent = string.Join("", Enumerable.Repeat("\t", count));
            return (first ? indent: "") + value.Replace("\n", "\n" + indent);
        }
        public static string[] Tokenized(this string value, bool allWhite = false, string separators = "\n\r")
        {
            if (allWhite)
            {
                separators = "\t\n\r ";
            }
            return value.Split(separators.ToArray())
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();
        }

        public static string Join (this IEnumerable<string> values, int intend = 0,  bool first = false, string separator = "\n")
        {
            return string.Join(separator, values).Intend(intend, first);
        }
       
    }
}