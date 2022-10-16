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
        static void Main(string[] args)
        {
            var inputPath = @"../../../UniqueWordsClient/WarAndPeace.txt";
            var outputPath = @"../../../UniqueWordsClient/UniqueWords.txt";

            while (true)
            {
                ReadAndWriteUsingReflection(inputPath, outputPath);
                Console.WriteLine();
                ReadAndWriteUsingMultiThreading(inputPath, outputPath);
                Console.WriteLine("\n---------------------------------");
                GetResultsFromService();

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

            var wordCounts = new TextParserAsync().BuildDictionary(inputPath);
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

        private static void GetResultsFromService()
        {
            //Set up dot instead of comma in numeric values
            System.Globalization.CultureInfo customCulture =
                (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            //Step 1: Create an instance of the WCF proxy.
            CalculatorClient client = new CalculatorClient();

            // Step 2: Call the service operations.
            // Call the Add service operation.
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            value1 = 145.00D;
            value2 = 76.54D;
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            value1 = 9.00D;
            value2 = 81.25D;
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            value1 = 22.00D;
            value2 = 7.00D;
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

            // Step 3: Close the client to gracefully close the connection and clean up resources.
            Console.WriteLine("\nPress <Enter> to terminate the client.");
            Console.ReadLine();
            client.Close();
        }
    }
}
