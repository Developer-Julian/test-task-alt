using System.Collections;
using System.Collections.Generic;

namespace TestTaskAlt.Sortet
{
    internal sealed class TargetFileSet : IEnumerable<TargetFile>
    {
        private readonly List<TargetFile> files = new List<TargetFile>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<TargetFile> GetEnumerator()
        {
            return ((IEnumerable<TargetFile>) this.files).GetEnumerator();
        }

        public void Add(TargetFile file)
        {
            files.Add(file);
        }
    }
}