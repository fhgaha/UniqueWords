using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueWordsLibrary
{
    public class TextParserAsyncThreeTimesSlower
    {
        private ConcurrentDictionary<string, int> _wordCounts =
             new ConcurrentDictionary<string, int>(Environment.ProcessorCount * 2, 15000);

        public IDictionary<string, int> BuildDictionary(string inputPath)
        {
            Parallel.ForEach(source: File.ReadLines(inputPath), body: AddWordsToDictionary);
            return _wordCounts;
        }

        private void AddWordsToDictionary(string line)
        {
            Parallel.ForEach(
                source: ParseWords(line),
                body: (word) => _wordCounts.AddOrUpdate(word, 1, (key, existingValue) => existingValue + 1));
        }

        private IEnumerable<string> ParseWords(string line) =>
           line.Split()
               .Select(word => word.ToLower().Trim(c => !char.IsLetter(c)))
               .Where(word => !string.IsNullOrEmpty(word));
    }
}
