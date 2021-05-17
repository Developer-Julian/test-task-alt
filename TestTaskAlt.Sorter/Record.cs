using System;

namespace TestTaskAlt.Sortet
{
    internal sealed class Record : IComparable<Record>
    {
        public Record(uint number, string text)
        {
            this.Number = number;
            this.Text = text;
        }

        public uint Number { get; private set; }
        public string Text { get; private set; }

        public int CompareTo(Record other)
        {
            var compareResult = string.Compare(this.Text, other.Text, StringComparison.InvariantCulture);
            if (compareResult == 0)
            {
                return this.Number.CompareTo(other.Number);
            }

            return compareResult;
        }

        public override string ToString()
        {
            return string.Concat(this.Number.ToString(), ". ", this.Text);
        }
    }
}