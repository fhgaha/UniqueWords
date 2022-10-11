using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UniqueWordsLibrary;

namespace UniqueWords
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputPath = @"../../../WarAndPeace.txt";
            var outputPath = @"../../../UniqueWords.txt";

            while (true)
            {
                Console.WriteLine("Processing...");
                var stopwatch = Stopwatch.StartNew();

                var wordAmounts = GetDictionary(inputPath);
                Write(wordAmounts, outputPath);

                Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds} milliseconds");
                Console.ReadLine();
            }
        }

        private static Dictionary<string, int> GetDictionary(string inputPath)
        {
            var type = typeof(MyTextParser);
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod("BuildDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var wordAmounts = (Dictionary<string, int>) method!.Invoke(instance, new object[] {inputPath});
            return wordAmounts;
        }

        private static void Write(Dictionary<string, int> wordCounts, string outputPath)
        {
            var longestWordLength = wordCounts!.Keys.Max(word => word.Length);
            int GetSpaceAmount(int wordLength) => longestWordLength - wordLength + 1;

            var output = wordCounts.OrderByDescending(pair => pair.Value)
                .Select(pair => string.Format("{0}{1}{2}",
                    pair.Key,
                    new string(' ', GetSpaceAmount(pair.Key.Length)),
                    pair.Value))
                .ToArray();

            File.WriteAllLines(outputPath, output);
        }
    }
}
