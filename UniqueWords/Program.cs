/*
2.
Задание по блоку Процессы и потоки:
В dll предыдущего задания реализовать публичный метод аналогичный приватному, но с многопоточной обработкой текста.
Сравнить время выполнения приватного и публичного методов при помощи объекта StopWach.
Вместе с кодом прислать время выполнения методов.

3.
Итоговое задание по блоку:
На основе публичного метода из задания "Процессы и потоки" реализовать WCF или WebAPI сервис, который на вход принимает текст, 
возвращает Dictionary<string, int>. Вместе с кодом сервиса должен присутствовать код приложения его вызывающий.
 */

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

            var wordCounts = new TextParserFaster().BuildDictionary(inputPath);
            Write(wordCounts, outputPath);

            stopwatch.Stop();
            Console.WriteLine($"    Done in {stopwatch.ElapsedMilliseconds} milliseconds");
        }

        private static void Write(IDictionary<string, int> wordCounts, string outputPath)
        {
            if (wordCounts.Count == 0)
            {
                Console.WriteLine("wordCounts is empty");
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
