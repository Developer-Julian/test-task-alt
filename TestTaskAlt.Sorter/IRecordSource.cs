namespace TestTaskAlt.Sortet
{
    internal interface IRecordSource
    {
        bool HasMoreRecords { get; }
        bool MoveToNextRunData();
        Record Read();
    }
}