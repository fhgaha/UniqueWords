using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniqueWordsLibrary
{
    public class TextParserUsingReflection
    {
        private IDictionary<string, int> _wordCounts;

        private IDictionary<string, int> BuildDictionary(string inputPath)
        {
            _wordCounts = new Dictionary<string, int>();

            foreach (string line in File.ReadLines(inputPath))
                AddWordsToDictionary(line);

            return _wordCounts;
        }

        private void AddWordsToDictionary(string line)
        {
            foreach (string word in ParseWords(line))
            {
                if (_wordCounts.ContainsKey(word)) 
                    _wordCounts[word]++;
                else 
                    _wordCounts.Add(word, 1);
            }
        }

        private IEnumerable<string> ParseWords(string line) =>
            line.Split()
                .Select(word => word.ToLower().Trim(c => !char.IsLetter(c)))
                .Where(word => !string.IsNullOrEmpty(word));
    }
}