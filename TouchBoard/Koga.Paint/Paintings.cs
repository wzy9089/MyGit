using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public class Paintings : IList<Painting>
    {
        private List<Painting> paintings = new List<Painting>();

        public Painting this[int index] { get => ((IList<Painting>)paintings)[index]; set => ((IList<Painting>)paintings)[index] = value; }

        public int Count => ((ICollection<Painting>)paintings).Count;

        public bool IsReadOnly => ((ICollection<Painting>)paintings).IsReadOnly;

        public void Add(Painting item)
        {
            ((ICollection<Painting>)paintings).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Painting>)paintings).Clear();
        }

        public bool Contains(Painting item)
        {
            return ((ICollection<Painting>)paintings).Contains(item);
        }

        public void CopyTo(Painting[] array, int arrayIndex)
        {
            ((ICollection<Painting>)paintings).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Painting> GetEnumerator()
        {
            return ((IEnumerable<Painting>)paintings).GetEnumerator();
        }

        public int IndexOf(Painting item)
        {
            return ((IList<Painting>)paintings).IndexOf(item);
        }

        public void Insert(int index, Painting item)
        {
            ((IList<Painting>)paintings).Insert(index, item);
        }

        public bool Remove(Painting item)
        {
            return ((ICollection<Painting>)paintings).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Painting>)paintings).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)paintings).GetEnumerator();
        }
    }
}
