using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public class Painting : IList<PaintingElement>
    {
        public event EventHandler<PaintintChangedEventArgs>? PaintingChanged;

        List<PaintingElement> elements = new List<PaintingElement>();

        private void OnPaintingChanged()
        {
            PaintingChanged?.Invoke(this, new PaintintChangedEventArgs());
        }

        #region Ilist implementation
        public PaintingElement this[int index] 
        { 
            get => ((IList<PaintingElement>)elements)[index];
            set
            {
                ((IList<PaintingElement>)elements)[index] = value;
                OnPaintingChanged();
            }
        }

        public int Count => ((ICollection<PaintingElement>)elements).Count;

        public bool IsReadOnly => ((ICollection<PaintingElement>)elements).IsReadOnly;

        public void Add(PaintingElement item)
        {
            ((ICollection<PaintingElement>)elements).Add(item);
            OnPaintingChanged();
        }

        public void Clear()
        {
            ((ICollection<PaintingElement>)elements).Clear();
            OnPaintingChanged();
        }

        public bool Contains(PaintingElement item)
        {
            return ((ICollection<PaintingElement>)elements).Contains(item);
        }

        public void CopyTo(PaintingElement[] array, int arrayIndex)
        {
            ((ICollection<PaintingElement>)elements).CopyTo(array, arrayIndex);
        }

        public IEnumerator<PaintingElement> GetEnumerator()
        {
            return ((IEnumerable<PaintingElement>)elements).GetEnumerator();
        }

        public int IndexOf(PaintingElement item)
        {
            return ((IList<PaintingElement>)elements).IndexOf(item);
        }

        public void Insert(int index, PaintingElement item)
        {
            ((IList<PaintingElement>)elements).Insert(index, item);
            OnPaintingChanged();
        }

        public bool Remove(PaintingElement item)
        {
            bool ret = ((ICollection<PaintingElement>)elements).Remove(item);
            if (ret)
                OnPaintingChanged();
            return ret;
        }

        public void RemoveAt(int index)
        {
            ((IList<PaintingElement>)elements).RemoveAt(index);
            OnPaintingChanged();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)elements).GetEnumerator();
        }
        #endregion
    }
}
