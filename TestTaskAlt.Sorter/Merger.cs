using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTaskAlt.Sortet
{
    internal class Merger : IRecordSource, IDisposable
    {
        private readonly List<SourceFile> sourceFiles;

        public Merger(IEnumerable<TargetFile> targetFiles)
        {
            this.sourceFiles = targetFiles.Select(targetFile => new SourceFile(targetFile)).ToList();
        }

        public void Dispose()
        {
            foreach (var file in this.sourceFiles)
            {
                file?.Dispose();
            }
        }

        public bool MoveToNextRunData()
        {
            var hasMoreRuns = false;
            foreach (var file in this.sourceFiles)
            {
                if (file.MoveToNextRun())
                {
                    hasMoreRuns = true;
                }
            }

            return hasMoreRuns;
        }

        public long PredictedSize => this.sourceFiles.Sum(file => file.FileSize);

        public bool HasMoreRecords => this.sourceFiles.Any(file => file.HasMoreRecords);

        public Record Read()
        {
            Record minRecord = null;
            SourceFile minRecordSource = null;
            foreach (var file in this.sourceFiles)
            {
                var topRecord = file.Peek();
                if (topRecord == null)
                {
                    continue;
                }

                if (minRecord == null || minRecord.CompareTo(topRecord) > 0)
                {
                    minRecord = topRecord;
                    minRecordSource = file;
                }
            }

            return minRecordSource?.Pop();
        }


        private class SourceFile : IDisposable
        {
            private readonly FileReader reader;
            private Record topRecord;

            public SourceFile(TargetFile targetFile)
            {
                this.reader = new FileReader(targetFile.TargetFilePath);
            }

            public bool HasMoreRecords => !this.reader.EndOfFile;

            public long FileSize => this.reader.TotalSizeInBytes;

            public void Dispose()
            {
                this.reader?.Dispose();
            }

            public bool MoveToNextRun()
            {
                return this.reader.MoveToNextRun();
            }

            public Record Peek()
            {
                if (this.topRecord == null)
                {
                    this.topRecord = this.ExtractTop();
                }

                return topRecord;
            }

            public Record Pop()
            {
                if (this.topRecord != null)
                {
                    var resultRecord = topRecord;
                    this.topRecord = this.ExtractTop();
                    return resultRecord;
                }
                else
                {
                    var resultRecord = this.ExtractTop();
                    this.topRecord = this.ExtractTop();
                    return resultRecord;
                }
            }

            private Record ExtractTop()
            {
                return this.reader.Read();
            }
        }
    }
}