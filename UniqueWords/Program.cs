using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueWords
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputPath = @"D:\MyProjects\VS 19 projects\ConsoleApps\UniqueWords\UniqueWords\WarAndPeace.txt";
            var outputPath = @"D:\MyProjects\VS 19 projects\ConsoleApps\UniqueWords\UniqueWords\UniqueWords.txt";

            while(true)
            {
                Console.WriteLine("Processing...");
                var stopwatch = Stopwatch.StartNew();
                File.WriteAllLines(outputPath, new MyTextParser().GetUniqueWords(inputPath));

                Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds} milliseconds");
                Console.ReadLine();
            }
        }
    }

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

        private async void AddWordsToDictionary(string line)
        {
            foreach (string word in await ParseWords(line))
            {
                if (!_wordCounts.ContainsKey(word))
                    _wordCounts.Add(word, 0);

                _wordCounts[word]++;
            }
        }

        private async Task<IEnumerable<string>> ParseWords(string line) =>
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

    public static class StringExtensions
    {
        public static string Trim(this string str, Func<char, bool> func) =>
            str.Where(c => !func(c))
                .Aggregate("", (current, c) => current + c);
    }
}
