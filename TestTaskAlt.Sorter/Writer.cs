using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTaskAlt.Sortet
{
    internal sealed class Writer : IDisposable
    {
        private int currentTarget;
        private readonly List<Target> targets = new List<Target>();
        private readonly IRecordSource recordSource;

        public Writer(IRecordSource recordSource)
        {
            this.recordSource = recordSource;
        }


        public int TotalRunsCount
        {
            get { return targets.Sum(target => target.RunsCount); }
        }

        public void Dispose()
        {
            foreach (var target in targets)
            {
                target.Dispose();
            }
        }

        public void AddTarget(TargetFile targetFile)
        {
            targets.Add(new Target(targetFile));
        }

        public List<TargetFile> GetUsedTargetFiles()
        {
            return targets.Where(target => target.RunsCount > 0)
                .Select(target => target.TargetFile).ToList();
        }

        public long Write()
        {
            if (targets.Count < 1)
            {
                throw new IOException("No targets were set");
            }

            var writeCount = 0L;
            while (this.recordSource.MoveToNextRunData())
            {
                var target = this.targets[currentTarget];
                var writer = target.BeginWrite();

                Record record;
                while ((record = this.recordSource.Read()) != null)
                {
                    writer.Write(record.Number + ". ");
                    writer.WriteLine(record.Text);
                    writeCount += 1;
                }

                target.EndWrite();

                this.currentTarget++;
                if (currentTarget >= targets.Count)
                {
                    this.currentTarget = 0;
                }
            }

            return writeCount;
        }

        private class Target : IDisposable
        {
            private readonly TextWriter writer;
            private readonly TargetFile targetFile;
            public int RunsCount
            {
                get { return TargetFile.RunsCount; }
                private set { TargetFile.RunsCount = value; }
            }

            public TargetFile TargetFile
            {
                get { return this.targetFile; }
            }

            public Target(TargetFile targetFile)
            {
                this.targetFile = targetFile;
                var stream = new FileStream(targetFile.TargetFilePath, FileMode.Create, FileAccess.Write,
                    FileShare.None, 1 << 18, FileOptions.SequentialScan);
                this.writer = new StreamWriter(stream);
                this.RunsCount = 0;
            }

            public void Dispose()
            {
                this.writer?.Close();
            }

            public TextWriter BeginWrite()
            {
                if (this.RunsCount >= 1)
                {
                    this.writer.WriteLine("===");
                }

                return this.writer;
            }

            public void EndWrite()
            {
                this.RunsCount++;
            }
        }
    }
}