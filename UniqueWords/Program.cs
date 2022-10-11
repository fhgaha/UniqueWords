using System;
using System.Diagnostics;
using System.IO;
using System.Text;

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
}
