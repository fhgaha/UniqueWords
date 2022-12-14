using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniqueWordsLibrary
{
    public class TextParserMultiThreading
    {
        private ConcurrentDictionary<string, int> _wordCounts =
             new ConcurrentDictionary<string, int>(Environment.ProcessorCount, 15000);

        public IDictionary<string, int> BuildDictionary(string inputPath)
        {
            Parallel.ForEach(source: File.ReadLines(inputPath, Encoding.UTF8), body: AddWordsToDictionary);
            return _wordCounts;
        }

        private void AddWordsToDictionary(string line)
        {
            foreach (var word in ParseWords(line))
                _wordCounts.AddOrUpdate(word, 1, (key, existingValue) => existingValue + 1);
        }

        private IEnumerable<string> ParseWords(string line) =>
           line.Split()
               .Select(word => word.ToLower().Trim(c => !char.IsLetter(c)))
               .Where(word => !string.IsNullOrEmpty(word));
    }
}
