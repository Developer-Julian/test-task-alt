using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace TestTaskAlt.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json", false)
            .Build();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Console.WriteLine("Input file size in Gb: ");
            var fileSizeStr = Console.ReadLine();

            var fileSize = double.Parse(fileSizeStr) * 1024L * 1024 * 1024;
            var fileGenerator = new FileGenerator(configuration, fileSize);
            fileGenerator.Process();
            Console.WriteLine("File is successfully generated. Please press any button to close the window...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
