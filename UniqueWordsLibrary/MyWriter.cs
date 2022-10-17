using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueWordsLibrary
{
    public static class MyWriter
    {
        public static void Write(IDictionary<string, int> wordCounts, string outputPath)
        {
            if (wordCounts.Count == 0)
            {
                Console.WriteLine("wordCounts is empty");
                return;
            }

            var longestWordLength = wordCounts.Keys.Max(word => word.Length);
            int GetSpaceAmount(int wordLength) => longestWordLength - wordLength + 1;

            var output = wordCounts
                .OrderByDescending(pair => pair.Value)
                .Select(pair => string.Format("{0}{1}{2}",
                    pair.Key,
                    new string(' ', GetSpaceAmount(pair.Key.Length)),
                    pair.Value))
                .ToArray();

            File.WriteAllLines(outputPath, output);
        }
    }
}
