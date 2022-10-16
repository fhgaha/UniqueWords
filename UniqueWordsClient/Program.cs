/*
1.
Задание по блоку Reflection:
Реализацию по вступительному заданию разделить на 2 сборки: exe и dll. 
Exe читает файл, вызывает приватный метод из dll, передает ему текст из файла, получает результат и записывает его в файл. 
Dll содержит 1 класс и приватный метод, который принимает на вход текст, возвращает Dictionary<string, int>
Задание по блоку Reflection со звездочкой:
при помощи рефлексии/IL вставок или любого другого механизма создать экземпляр абстрактного класса. 
Без переопределения, без наследования. Рекомендую заглянуть на https://referencesource.microsoft.com/

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
using UniqueWordsClient.ServiceReference1;
using UniqueWordsLibrary;

namespace UniqueWordsClient
{
    class Program
    {
        private const string inputPath = @"D:\MyProjects\VS 19 projects\ConsoleApps\UniqueWords\UniqueWordsClient\WarAndPeace.txt";
        private const string outputPath = @"D:\MyProjects\VS 19 projects\ConsoleApps\UniqueWords\UniqueWordsClient\UniqueWords.txt";

        static void Main(string[] args)
        {
            //var inputPath = @"../../../UniqueWordsClient/WarAndPeace.txt";
            //var outputPath = @"../../../UniqueWordsClient/UniqueWords.txt";

            while (true)
            {
                ReadAndWriteUsingReflection(inputPath, outputPath);
                Console.WriteLine();
                ReadAndWriteUsingMultiThreading(inputPath, outputPath);
                Console.WriteLine();
                GetResultsFromService(inputPath);
                Console.WriteLine("\n---------------------------------");

                Console.WriteLine("Press <Enter> to read and write again.");
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
            var wordAmounts = (Dictionary<string, int>)method.Invoke(instance, new object[] { inputPath });
            return wordAmounts;
        }

        private static void ReadAndWriteUsingMultiThreading(string inputPath, string outputPath)
        {
            Console.WriteLine("Read and write using multithreading:");
            Console.WriteLine("    Processing...");
            var stopwatch = Stopwatch.StartNew();

            var wordCounts = new TextParserMultithreading().BuildDictionary(inputPath);
            Write(wordCounts, outputPath);

            stopwatch.Stop();
            Console.WriteLine($"    Done in {stopwatch.ElapsedMilliseconds} milliseconds");
        }

        private static void Write(IDictionary<string, int> wordCounts, string outputPath, int amount = 0)
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

            File.WriteAllLines(outputPath, amount == 0 ? output : output.Take(amount));
        }

        private static void GetResultsFromService(string inputPath)
        {
            WordCounterClient client = new WordCounterClient();

            Console.WriteLine("Read and write using web service:");
            Console.WriteLine("    Processing...");
            var stopwatch = Stopwatch.StartNew();

            var result = client.GetWordCountsAsync(inputPath).Result;
            Write(result, outputPath);

            stopwatch.Stop();
            Console.WriteLine($"    Done in {stopwatch.ElapsedMilliseconds} milliseconds");

            Console.WriteLine("\nPress <Enter> to terminate the client.");
            Console.ReadLine();
            client.Close();
        }
    }
}
