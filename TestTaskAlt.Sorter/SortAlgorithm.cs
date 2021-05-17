using System;
using System.Linq;
using System.Threading;

namespace TestTaskAlt.Sortet
{
    internal class SortAlgorithm
    {
        private TargetFileSet inputTargetFileSet;
        private TargetFileSet outputTargetFileSet;
        private readonly string sourceFilePath;

        public SortAlgorithm(string sourceFilePath)
        {
            this.sourceFilePath = sourceFilePath;
            this.inputTargetFileSet = new TargetFileSet { new TargetFile("ta1"), new TargetFile("ta2") };
            this.outputTargetFileSet = new TargetFileSet { new TargetFile("tb1"), new TargetFile("tb2") };
        }

        public void Run()
        {
            MemoryInfo.Reset();
            using (var sorter = new Sorter(new FileReader(this.sourceFilePath)))
            using (var writer = new Writer(sorter))
            {
                foreach (var targetFile in this.outputTargetFileSet)
                {
                    writer.AddTarget(targetFile);
                }

                var writtenCount = writer.Write();
                Console.WriteLine($"{string.Join(", ", this.outputTargetFileSet.Select(file => file.TargetFilePath))}: Total number of written records: {writtenCount}");
            }

            GC.Collect();
            while (this.outputTargetFileSet.Sum(file => file.RunsCount) > 1)
            {
                this.SwitchTargetRoles();
                using (var mrg = new Merger(this.inputTargetFileSet))
                using (var writer = new Writer(mrg))
                {
                    foreach (var targetFile in this.outputTargetFileSet)
                    {
                        writer.AddTarget(targetFile);
                    }

                    var writtenCount = writer.Write();
                    Console.WriteLine($"Total number of written records: {writtenCount}");
                }
            }

            var finalOutputFile = this.outputTargetFileSet.FirstOrDefault(file => file.RunsCount == 1) ??
                                  this.outputTargetFileSet.First();

            finalOutputFile.MakeFinal();
        }

        private void SwitchTargetRoles()
        {
            this.outputTargetFileSet = Interlocked.Exchange(ref inputTargetFileSet, outputTargetFileSet);
        }
    }
}