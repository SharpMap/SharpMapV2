using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    public class CollectionBase<T> : CollectionBase
    {
        #region Delegates

        public delegate bool EnsureUniqueDelegate(T item, T check);

        #endregion

        private readonly EnsureUniqueDelegate _ensureUnique;

        public CollectionBase(EnsureUniqueDelegate uniqueChecker) : this(uniqueChecker, null)
        {
        }


        public CollectionBase(EnsureUniqueDelegate uniqueChecker, IEnumerable<T> items)
        {
            _ensureUnique = uniqueChecker;
            AddRange(items);
        }

        public T this[int i]
        {
            get { return (T) List[i]; }
            set { List[i] = value; }
        }

        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoved;

        public virtual IEnumerable<TLookFor> FindByType<TLookFor>() where TLookFor : T
        {
            foreach (T component in List)
                if (component is TLookFor)
                    yield return (TLookFor) component;
        }

        public virtual void Add(T item)
        {
            if (!EnsureUniqueId(item))
                throw new DuplicateNameException();

            List.Add(item);
            OnItemAdded(new ItemEventArgs(item));
        }

        private void OnItemAdded(ItemEventArgs itemEventArgs)
        {
            if (ItemAdded != null)
                ItemAdded(this, itemEventArgs);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (Equals(null, items))
                return;

            foreach (T item in items)
                Add(item);
        }

        private bool EnsureUniqueId(T item)
        {
            foreach (T component in List)
                if (!_ensureUnique(component, item))
                    return false;
            return true;
        }

        public virtual void Insert(int ndx, T item)
        {
            List.Insert(ndx, item);
        }

        public virtual void Remove(T item)
        {
            List.Remove(item);

            OnItemRemoved(new ItemEventArgs(item));
        }

        private void OnItemRemoved(ItemEventArgs itemEventArgs)
        {
            if (ItemRemoved != null)
                ItemRemoved(this, itemEventArgs);
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void CopyTo(T[] array, int index)
        {
            List.CopyTo(array, index);
        }

        #region Nested type: ItemEventArgs

        public class ItemEventArgs : EventArgs
        {
            internal ItemEventArgs(T item)
            {
                Item = item;
            }

            public T Item { get; protected set; }
        }

        #endregion
    }
}