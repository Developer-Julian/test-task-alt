using System;
using System.Diagnostics;
using System.IO;

namespace TestTaskAlt.Sortet
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to the file you want to sort");
            var filePath = Console.ReadLine();
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error. Can't find hte file by path: {filePath}");
                Environment.Exit(1);
            }

            try
            {
                var watch = new Stopwatch();
                watch.Start();
                var sortAlgorithm = new SortAlgorithm(filePath);
                sortAlgorithm.Run();
                Console.WriteLine($"File was sorted in {watch.Elapsed.TotalSeconds} seconds");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}