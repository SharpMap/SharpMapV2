using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable enumerable)
        {
            foreach (object o in enumerable)
            {
                if (o is T)
                    yield return (T) o;
            }
        }

        public static int RawIndexOf(this IEnumerable enumerable, object item)
        {
            int i = -1;
            foreach (object o in enumerable)
            {
                i++;
                if (o == item)
                    return i;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable enumerable, T item)
        {
            return enumerable.AsEnumerable<T>().RawIndexOf(item);
        }

        public static object ItemAtRawIndex(this IEnumerable enumerable, int index)
        {
            if (index < 0)
                throw new ArgumentException();

            int i = 0;
            foreach (object v in enumerable)
            {
                if (i == index)
                    return v;
                i++;
            }
            return null;
        }

        public static T ItemAtIndex<T>(this IEnumerable enumerable, int index)
        {
            return (T) enumerable.AsEnumerable<T>().ItemAtRawIndex(index);
        }

        public static IEnumerable<int> RawIndexes<T>(this IEnumerable enumerable)
        {
            int i = -1;
            foreach (object o in enumerable)
            {
                i++;
                if (o is T)
                    yield return i;
            }
        }
    }

    public class ChildControlCollection<TControl> : IList, IList<TControl> where TControl : Control
    {
        private readonly ControlCollection _internalCollection;

        public ChildControlCollection(ControlCollection parentControlCollection)
        {
            _internalCollection = parentControlCollection;
        }

        #region IList Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RemoveAt(int index)
        {
            Remove(_internalCollection.ItemAtIndex<TControl>(index));
        }

        public void Clear()
        {
            foreach (TControl v in this.Reverse())
                Remove(v);
        }

        public int Count
        {
            get { return _internalCollection.AsEnumerable<TControl>().Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Add(object value)
        {
            GuardType(value);
            Add((TControl) value);
            return IndexOf((TControl) value);
        }

        public bool Contains(object value)
        {
            GuardType(value);
            return Contains((TControl) value);
        }

        public int IndexOf(object value)
        {
            GuardType(value);
            return IndexOf((TControl) value);
        }

        public void Insert(int index, object value)
        {
            GuardType(value);
            Insert(index, (TControl) value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            GuardType(value);
            Remove((TControl) value);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotImplementedException(); }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IList<TControl> Members

        public IEnumerator<TControl> GetEnumerator()
        {
            foreach (TControl c in _internalCollection.AsEnumerable<TControl>())
                yield return c;
        }

        public int IndexOf(TControl item)
        {
            return _internalCollection.AsEnumerable<TControl>().IndexOf(item);
        }

        public void Insert(int index, TControl item)
        {
            int rawIndex = index == 0
                               ? 0
                               : _internalCollection.RawIndexOf(_internalCollection.ItemAtIndex<TControl>(index));

            _internalCollection.AddAt(rawIndex, item);
            OnItemAdded(new ChildControlEventArgs(item));
        }

        public TControl this[int index]
        {
            get { return _internalCollection.ItemAtIndex<TControl>(index); }
            set { throw new NotImplementedException(); }
        }

        public void Add(TControl item)
        {
            _internalCollection.Add(item);
            OnItemAdded(new ChildControlEventArgs(item));
        }

        public bool Contains(TControl item)
        {
            return _internalCollection.IndexOf(item) > -1;
        }

        public void CopyTo(TControl[] array, int arrayIndex)
        {
            _internalCollection.AsEnumerable<TControl>().ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(TControl item)
        {
            int i = _internalCollection.IndexOf(item);
            if (i < 0)
                return false;
            _internalCollection.RemoveAt(i);
            OnItemRemoved(new ChildControlEventArgs(item));
            return true;
        }

        #endregion

        public event EventHandler<ChildControlEventArgs> ItemAdded;
        public event EventHandler<ChildControlEventArgs> ItemRemoved;

        private void OnItemRemoved(ChildControlEventArgs args)
        {
            if (ItemRemoved != null)
                ItemRemoved(this, args);
        }

        private void OnItemAdded(ChildControlEventArgs args)
        {
            if (ItemAdded != null)
                ItemAdded(this, args);
        }

        private static void GuardType(object o)
        {
            if (!(o is TControl))
                throw new ArgumentException("invalid object type");
        }

        #region Nested type: ChildControlEventArgs

        public class ChildControlEventArgs : EventArgs
        {
            internal ChildControlEventArgs(TControl item)
            {
                Control = item;
            }

            public TControl Control { get; protected set; }
        }

        #endregion
    }
}