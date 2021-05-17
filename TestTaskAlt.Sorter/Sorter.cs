using System;

namespace TestTaskAlt.Sortet
{
    internal class Sorter : IRecordSource, IDisposable
    {
        private static readonly int BatchSize = 100000;
        private readonly FileReader reader;
        private BinaryHeap<Record> runRecords;

        public Sorter(FileReader reader)
        {
            this.reader = reader;
        }

        public void Dispose()
        {
            this.reader?.Dispose();
        }

        public bool MoveToNextRunData()
        {
            return this.ReadNextRunData();
        }

        public bool HasMoreRecords => !this.reader.EndOfFile;

        public Record Read()
        {
            if (this.runRecords == null)
            {
                this.ReadNextRunData();
            }

            if (this.runRecords.Count > 0)
            {
                return this.runRecords.RemoveRoot();
            }

            return null;
        }

        private bool ReadNextRunData()
        {
            this.runRecords = new BinaryHeap<Record>((int)this.reader.TotalSizeInBytes/512);
            GC.Collect();

            this.reader.ResumeReading();
            while (true)
            {
                for (var i = 0; i < BatchSize; i++)
                {
                    var record = this.reader.Read();
                    if (record == null)
                    {
                        return this.StopReading();
                    }

                    runRecords.Insert(record);
                }

                if (
                    MemoryInfo.GetOccupiedMemoryPercent() >= 0.8 && 
                    MemoryInfo.GetFreeMemoryLeft() < 1024 * 1024 * 1024
                )
                {
                    this.reader.PauseReading();
                }
            }
        }

        private bool StopReading() {
            if (this.runRecords.Count < 1)
            {
                return false;
            }

            return true;
        }
    }
}