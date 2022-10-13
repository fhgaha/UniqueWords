using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueWordsLibrary
{
    public class SpeedMeasurer
    {
        private const string InputPath = @"../../../WarAndPeace.txt";
        private const string OutputPath = @"../../../UniqueWords.txt";

        public void Run()
        {
            GetFileReadAllLinesSpeed();
            GetBufferedAndReaderStreamSpeed();
        }

        public static void GetBufferedAndReaderStreamSpeed()
        {
            var sw = new Stopwatch();
            Console.WriteLine("---start reading using streams");
            sw.Start();

            using (FileStream fs = File.OpenRead(InputPath))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                }
            }

            sw.Stop();
            Console.WriteLine("---done reading using streams. elapsed {0} ms", sw.ElapsedMilliseconds);
            sw.Reset();
        }

        public static void GetFileReadAllLinesSpeed()
        {
            var sw = new Stopwatch();
            Console.WriteLine("---start reading using File.ReadAllLines");
            sw.Start();

            var text = File.ReadAllLines(InputPath);

            sw.Stop();
            Console.WriteLine("---done reading using File.ReadAllLines. elapsed {0} ms", sw.ElapsedMilliseconds);
            sw.Reset();
        }
    }
}
