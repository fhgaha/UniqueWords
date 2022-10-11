using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniqueWords
{
    class MyTextParser
    {
        private IDictionary<string, int> _wordCounts;

        public string[] GetUniqueWords(string input) => 
            BuildDictionary(input).ToArray();

        private MyTextParser BuildDictionary(string inputPath)
        {
            _wordCounts = new Dictionary<string, int>();

            foreach (string line in File.ReadLines(inputPath))
                AddWordsToDictionary(line);

            return this;
        }

        private void AddWordsToDictionary(string line)
        {
            foreach (string word in ParseWords(line))
            {
                if (!_wordCounts.ContainsKey(word))
                    _wordCounts.Add(word, 0);

                _wordCounts[word]++;
            }
        }

        private IEnumerable<string> ParseWords(string line) =>
            line.Split()
                .Select(word => word.ToLower().Trim(c => !char.IsLetter(c)))
                .Where(word => !string.IsNullOrEmpty(word));

        public string[] ToArray()
        {
            var padValue = _wordCounts.Keys.Max(word => word.Length);

            return _wordCounts.OrderByDescending(pair => pair.Value)
                .Select(pair => string.Format("{0}{1}{2}", 
                    pair.Key, 
                    new string(' ', padValue - pair.Key.Length + 1),
                    pair.Value))
                .ToArray();
        }
    }
}