using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace _03_ThreadSynchronization
{
    public class Program
    {
        class Stat
        {
            public int Words { get; set; }
            public int Lines { get; set; }
            public int Punctuation { get; set; }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            string directoryPath = @"C:\Users\Olia\Desktop\промбудова";
            string[] files = Directory.GetFiles(directoryPath);

            var result = new Stat();
            object lockObject = new object();

            Thread[] threads = new Thread[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                string info = File.ReadAllText(path);

                threads[i] = new Thread((contentObj) =>
                {
                    string content = (string)contentObj;

                    var statistics = new Stat();
                    statistics.Words = content.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    statistics.Lines = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    statistics.Punctuation = content.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .SelectMany(c => c)
                        .Count(char.IsPunctuation);

                    lock (result)
                    {
                        result.Words += statistics.Words;
                        result.Lines += statistics.Lines;
                        result.Punctuation += statistics.Punctuation;
                    }


                    Console.WriteLine($"File: {Path.GetFileName(path), -50} Words: {statistics.Words, -7} Lines: {statistics.Lines, -7} Punctuation: {statistics.Punctuation}");

                });
                threads[i].Start(info);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine($"\nTOTAL >> Words: {result.Words,-7} Lines: {result.Lines, -7} Punctuation: {result.Punctuation}");

            Console.WriteLine("\n");
        }
    }
}
