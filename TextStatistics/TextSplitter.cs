using System;
using System.Linq;
using System.Text;

namespace TextStatistics
{
    public class TextSplitter
    {
        private static readonly char[] Punctuations = new[]
        {
            ',',
            '!',
            '?',
            '.',
            ':',
            '(',
            ')',
            '"',
            ';',
            '«',
            '»'
        };

        private const char Space = ' ';

        public string[] SplitWords(string source)
        {
            var groomedString = Punctuations.Aggregate(new StringBuilder(source), (current, item) => current.Replace(item, Space));

            return groomedString.ToString().Split(new[] { Space }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}