using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
                ReadAndWriteUsingReflection(inputPath, outputPath);
                Console.WriteLine();
                ReadAndWriteAsync(inputPath, outputPath);
                Console.WriteLine("\n---------------------------------");

                Console.ReadLine();
            }
        }

        private static void ReadAndWriteUsingReflection(string inputPath, string outputPath)
        {
            Console.WriteLine("Read and write using reflection:");
            Console.WriteLine("    Processing...");
            var stopwatch = Stopwatch.StartNew();

            var wordAmounts = GetDictionary(inputPath);
            Write(wordAmounts, outputPath);

            stopwatch.Stop();
            Console.WriteLine($"    Done in {stopwatch.ElapsedMilliseconds} milliseconds");
        }

        private static Dictionary<string, int> GetDictionary(string inputPath)
        {
            var type = typeof(TextParserUsingReflection);
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod("BuildDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var wordAmounts = (Dictionary<string, int>)method!.Invoke(instance, new object[] { inputPath });
            return wordAmounts;
        }

        private static void ReadAndWriteAsync(string inputPath, string outputPath)
        {
            Console.WriteLine("Read and write asynchronously:");
            Console.WriteLine("    Processing...");
            var stopwatch = Stopwatch.StartNew();

            var wordCounts = new TextParserAsync().BuildDictionary(inputPath);
            Write(wordCounts, outputPath);

            stopwatch.Stop();
            Console.WriteLine($"    Done in {stopwatch.ElapsedMilliseconds} milliseconds");
        }

        private static void Write(IDictionary<string, int> wordCounts, string outputPath)
        {
            if (wordCounts.Count == 0)
            {
                Console.WriteLine("wordCounts is empty", ConsoleColor.Red);
                return;
            }

            var longestWordLength = wordCounts!.Keys.Max(word => word.Length);
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
