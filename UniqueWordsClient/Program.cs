/*
1.
Использовать словарь.

2.
Задание по блоку Reflection:
Реализацию по вступительному заданию разделить на 2 сборки: exe и dll. 
Exe читает файл, вызывает приватный метод из dll, передает ему текст из файла, получает результат и записывает его в файл. 
Dll содержит 1 класс и приватный метод, который принимает на вход текст, возвращает Dictionary<string, int>
Задание по блоку Reflection со звездочкой:
при помощи рефлексии/IL вставок или любого другого механизма создать экземпляр абстрактного класса. 
Без переопределения, без наследования. Рекомендую заглянуть на https://referencesource.microsoft.com/

3.
Задание по блоку Процессы и потоки:
В dll предыдущего задания реализовать публичный метод аналогичный приватному, но с многопоточной обработкой текста.
Сравнить время выполнения приватного и публичного методов при помощи объекта StopWach.
Вместе с кодом прислать время выполнения методов.

4.
Итоговое задание по блоку:
На основе публичного метода из задания "Процессы и потоки" реализовать WCF или WebAPI сервис, который на вход принимает текст, 
возвращает Dictionary<string, int>. Вместе с кодом сервиса должен присутствовать код приложения его вызывающий.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UniqueWordsClient.ServiceReference1;

namespace UniqueWordsClient
{
    class Program
    {
        private const string InputPath = @"D:\MyProjects\VS 19 projects\ConsoleApps\UniqueWords\UniqueWordsClient\WarAndPeace.txt";
        private const string OutputPath = @"D:\MyProjects\VS 19 projects\ConsoleApps\UniqueWords\UniqueWordsClient\UniqueWords.txt";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("---------------------------------\n");

                ReadAndWriteUsing("reflection", DoReflection);
                ReadAndWriteUsing("multithreading", DoMultithreading);
                ReadAndWriteUsingWebService();

                Console.WriteLine("Press <Enter> to read and write again.");
                Console.ReadLine();
            }
        }

        private static void ReadAndWriteUsing(string approach, Action action = null)
        {
            Console.WriteLine($"Read and write using {approach}:");
            Console.WriteLine("    Processing...");
            var stopwatch = Stopwatch.StartNew();

            action();

            stopwatch.Stop();
            Console.WriteLine($"    Done in {stopwatch.ElapsedMilliseconds} milliseconds");
            Console.WriteLine();
        }

        private static void DoReflection()
        {
            var wordCounts = GetDictionaryUsingReflection(InputPath);
            UniqueWordsLibrary.MyWriter.Write(wordCounts, OutputPath);
        }

        private static Dictionary<string, int> GetDictionaryUsingReflection(string inputPath)
        {
            const string libPath = @"D:\MyProjects\VS 19 projects\ConsoleApps\UniqueWords\UniqueWordsLibrary\bin\Debug\net472\UniqueWordsLibrary.dll";
            var assembly = Assembly.LoadFrom(libPath);
            var type = assembly.GetType("UniqueWordsLibrary.TextParserUsingReflection");
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod("BuildDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var wordAmounts = (Dictionary<string, int>)method.Invoke(instance, new object[] { inputPath });
            return wordAmounts;
        }

        private static void DoMultithreading()
        {
            var wordCounts = new UniqueWordsLibrary.TextParserMultiThreading().BuildDictionary(InputPath);
            UniqueWordsLibrary.MyWriter.Write(wordCounts, OutputPath);
        }

        private static void ReadAndWriteUsingWebService()
        {
            var client = new WordCounterClient();

            ReadAndWriteUsing("web service", () => DoWebService(client));

            Console.WriteLine("\nPress <Enter> to terminate the client.");
            Console.ReadLine();
            client.Close();
        }

        private static void DoWebService(WordCounterClient client)
        {
            var wordCounts = client.GetWordCountsAsync(InputPath).Result;
            UniqueWordsLibrary.MyWriter.Write(wordCounts, OutputPath);
        }
    }
}
