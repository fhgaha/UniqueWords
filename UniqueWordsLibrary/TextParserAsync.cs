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
    public class TextParserAsync
    {
        private readonly ConcurrentDictionary<string, int> _wordCounts =
             new ConcurrentDictionary<string, int>(Environment.ProcessorCount * 2, 15000);

        public IDictionary<string, int> BuildDictionary(string inputPath)
        {
            using (FileStream fs = File.OpenRead(inputPath))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    AddWordsToDictionary(line);
                }
            }

            return _wordCounts;
        }

        private void AddWordsToDictionary(string line)
        {
            Parallel.ForEach(
                source: ParseWords(line),
                body: (word) => _wordCounts.AddOrUpdate(word, 1, (key, existingValue) => existingValue + 1));
        }

        private IEnumerable<string> ParseWords(string line)
        {
            Regex lettersOnly = new Regex(@"\W+");

            return lettersOnly.Split(line)
                .Select(word => word.ToLower())
                .Where(word => !string.IsNullOrEmpty(word));
        }
    }
}
