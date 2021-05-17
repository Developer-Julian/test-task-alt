using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace TestTaskAlt.Sortet
{
    internal sealed class FileReader : IDisposable
    {
        private const int BatchSize = 100000;
        private readonly ManualResetEvent moveToNextRunEvent;
        private readonly ManualResetEvent pauseReadingEvent;
        private readonly StreamReader reader;
        private volatile bool pauseReading;
        private Thread readingThread;

        public long TotalSizeInBytes { get; private set; }

        public bool EndOfFile
        {
            get { return Queue.IsAddingCompleted; }
        }

        private BlockingCollection<Record> Queue { get; }

        public FileReader(string filePath)
        {
            var fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 18, FileOptions.SequentialScan);
            this.TotalSizeInBytes = fstream.Length;
            this.reader = new StreamReader(fstream);
            this.pauseReadingEvent = new ManualResetEvent(true);
            this.moveToNextRunEvent = new ManualResetEvent(true);
            this.Queue = new BlockingCollection<Record>(BatchSize * 2);
            this.StartReading();
        }

        public void Dispose()
        {
            this.reader?.Dispose();
            this.pauseReadingEvent?.Dispose();
            this.readingThread?.Interrupt();
        }

        public bool MoveToNextRun()
        {
            if (!this.EndOfFile)
            {
                this.moveToNextRunEvent.Set();
                return true;
            }

            return false;
        }

        private void StartReading()
        {
            if (this.readingThread != null)
            {
                return;
            }

            this.readingThread = new Thread(() =>
            {
                while (true)
                {
                    WaitHandle.WaitAll(new WaitHandle[] {this.moveToNextRunEvent, this.pauseReadingEvent});
                    for (var i = 0; i < BatchSize; i++)
                    {
                        var line = this.reader.ReadLine();

                        if (line == "===")
                        {
                            this.moveToNextRunEvent.Reset();
                            break;
                        }

                        if (string.IsNullOrEmpty(line))
                        {
                            if (this.reader.EndOfStream)
                            {
                                break;
                            }

                            continue;
                        }
                        
                        var separatorPos = line.IndexOf(". ", StringComparison.InvariantCulture);
                        this.Queue.Add(new Record(
                            uint.Parse(line.Substring(0, separatorPos)),
                            line.Substring(separatorPos + 2)
                            ));
                    }

                    if (this.reader.EndOfStream)
                    {
                        break;
                    }
                }

                if (this.reader.EndOfStream)
                {
                    this.Queue.CompleteAdding();
                }
            });
            this.readingThread.Priority = ThreadPriority.AboveNormal;
            this.readingThread.Start();
        }

        public void ResumeReading()
        {
            this.pauseReadingEvent.Set();
            this.pauseReading = false;
        }

        public void PauseReading()
        {
            this.pauseReadingEvent.Reset();
            this.pauseReading = true;
        }

        public Record Read()
        {
            if (!this.Queue.IsCompleted)
            {
                try
                {
                    var spinWait = new SpinWait();
                    Record record = null;
                    while (!this.Queue.IsCompleted && !this.Queue.TryTake(out record))
                    {
                        if (this.pauseReading || !this.moveToNextRunEvent.WaitOne(0))
                        {
                            return null;
                        }

                        spinWait.SpinOnce();
                    }

                    return record;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}