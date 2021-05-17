using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskAlt.Generator
{
    public class FileGenerator
    {
        private readonly string fileName;
        private readonly string loremIpsum;
        private readonly string[] stringsToRepeat;
        private readonly double fileSize;
        private readonly object lockObject = new object();
        public FileGenerator(IConfigurationRoot configuration, double fileSize)
        {
            this.fileName = configuration.GetSection("FileName").Value;
            this.loremIpsum = configuration.GetSection("LoremIpsum").Value;
            this.stringsToRepeat = configuration.GetSection("StringsToRepeat").Value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            this.fileSize = fileSize;
        }

        public void Process()
        {
            var randomInt = new Random();

            var repeatCount = stringsToRepeat.Length * 3;
            var totalBytes = 0;
            using (var writer = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous)))
            {
                for (var i = 0; i < repeatCount; i++)
                {
                    var str = stringsToRepeat[randomInt.Next(stringsToRepeat.Length)];
                    var number = randomInt.Next(100);
                    var fileStr = $"{number}. {str}";
                    writer.WriteLine(fileStr);
                    totalBytes += fileStr.Length;
                }

                Parallel.For(0, 40, new ParallelOptions { MaxDegreeOfParallelism = 10 }, i =>
                {
                    while (totalBytes < fileSize)
                    {
                        var str = loremIpsum.Substring(randomInt.Next(loremIpsum.Length / 3));
                        var number = randomInt.Next(10000);
                        var fileStr = $"{number}. {str}";
                        lock (lockObject)
                        {
                            writer.WriteLine(fileStr);
                        }

                        totalBytes += fileStr.Length;
                    }
                });
            }
        }
    }
}
