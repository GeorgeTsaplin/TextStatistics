using System.Collections.Generic;
using System.Linq;

namespace TextStatistics
{
    public class WordsCalculator
    {
        public Dictionary<string, uint> Calculate(string[] words, IEqualityComparer<string> wordsComparer)
        {
            return words.Select(x => x.Trim())
                .GroupBy(
                    x => x,
                    (key, list) => new KeyValuePair<string, uint>(key, (uint) list.Count()),
                    wordsComparer)
                .ToDictionary(k => k.Key, v => v.Value, wordsComparer);
        }
    }
}
