/*
 *  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
 *  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    public class CollectionBase<T> : CollectionBase, IList<T>
    {
        #region Delegates

        public delegate bool EnsureUniqueDelegate(T item, T check);

        #endregion

        private readonly EnsureUniqueDelegate _ensureUnique;

        public CollectionBase(EnsureUniqueDelegate uniqueChecker)
            : this(uniqueChecker, null)
        {
        }


        public CollectionBase(EnsureUniqueDelegate uniqueChecker, IEnumerable<T> items)
        {
            _ensureUnique = uniqueChecker;
            AddRange(items);
        }

        #region IList<T> Members

        public T this[int i]
        {
            get { return (T) List[i]; }
            set { List[i] = value; }
        }

        public virtual void Add(T item)
        {
            if (!EnsureUniqueId(item))
                throw new DuplicateNameException();

            List.Add(item);
            OnItemAdded(new ItemEventArgs(item));
        }

        public virtual void Insert(int ndx, T item)
        {
            List.Insert(ndx, item);
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

        public new IEnumerator<T> GetEnumerator()
        {
            foreach (T item in List)
                yield return item;
        }

        public bool IsReadOnly
        {
            get { return List.IsReadOnly; }
        }

        bool ICollection<T>.Remove(T item)
        {
            if (!Contains(item))
                return false;
            Remove(item);
            return true;
        }

        #endregion

        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoved;

        public virtual IEnumerable<TLookFor> FindByType<TLookFor>() where TLookFor : T
        {
            foreach (T component in List)
                if (component is TLookFor)
                    yield return (TLookFor) component;
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

    public class DuplicateNameException : Exception
    {
    }
}