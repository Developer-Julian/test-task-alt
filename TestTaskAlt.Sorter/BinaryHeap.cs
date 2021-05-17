using System;
using System.Collections;
using System.Collections.Generic;

namespace TestTaskAlt.Sortet
{
    public class BinaryHeap<T> : IEnumerable<T>
    {
        private readonly IComparer<T> comparer;
        private readonly List<T> items;

        public BinaryHeap()
            : this(0, Comparer<T>.Default)
        {
        }

        public BinaryHeap(int capacity)
            : this(capacity, Comparer<T>.Default)
        {
        }

        public BinaryHeap(int capacity, IComparer<T> comp)
        {
            this.comparer = comp;
            this.items = new List<T>(capacity);
        }

        public int Count
        {
            get { return this.items.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Insert(T newItem)
        {
            var i = Count;
            this.items.Add(newItem);
            while (i > 0 && this.comparer.Compare(items[(i - 1)/2], newItem) > 0)
            {
                this.items[i] = this.items[(i - 1)/2];
                i = (i - 1)/2;
            }

            this.items[i] = newItem;
        }

        public T RemoveRoot()
        {
            if (this.items.Count == 0)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            var rslt = this.items[0];
            var tmp = this.items[items.Count - 1];
            this.items.RemoveAt(this.items.Count - 1);
            if (this.items.Count <= 0)
            {
                return rslt;
            }

            var i = 0;
            while (i < this.items.Count / 2)
            {
                var j = 2 * i + 1;
                if ((j < this.items.Count - 1) && (this.comparer.Compare(this.items[j], this.items[j + 1]) > 0))
                {
                    ++j;
                }

                if (this.comparer.Compare(this.items[j], tmp) >= 0)
                {
                    break;
                }

                this.items[i] = this.items[j];
                i = j;
            }

            this.items[i] = tmp;

            return rslt;
        }
    }
}