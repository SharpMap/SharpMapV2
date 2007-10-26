// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using SharpMap.Data;

namespace SharpMap.Data
{
    /// <summary>
    /// Represents the collection of tables for the FeatureDataSet.
    /// </summary>
    [Serializable]
    public class FeatureTableCollection : IList<FeatureDataTable>
    {
        private readonly DataTableCollection _tables;

        public FeatureTableCollection(DataTableCollection tables)
        {
            _tables = tables;
            _tables.CollectionChanged += tablesChanged;
            _tables.CollectionChanging += tablesChanging;
        }

        public event CollectionChangeEventHandler CollectionChanged;
        public event CollectionChangeEventHandler CollectionChanging;

        public FeatureDataTable this[String name]
        {
            get { return _tables[name] as FeatureDataTable; }
        }

        public FeatureDataTable this[String name, String tableNamespace]
        {
            get { return _tables[name, tableNamespace] as FeatureDataTable; }
        }

        #region IList<FeatureDataTable> Members

        public Int32 IndexOf(FeatureDataTable item)
        {
            return _tables.IndexOf(item);
        }

        public void RemoveAt(Int32 index)
        {
            _tables.RemoveAt(index);
        }

        public FeatureDataTable this[Int32 index]
        {
            get { return _tables[index] as FeatureDataTable; }
        }

        #endregion

        #region ICollection<FeatureDataTable> Members

        public void Add(FeatureDataTable item)
        {
            _tables.Add(item);
        }

        public void Clear()
        {
            _tables.Clear();
        }

        public bool Contains(FeatureDataTable item)
        {
            if (item == null) throw new ArgumentNullException("item");
            return _tables.Contains(item.TableName);
        }

        public bool Contains(String name)
        {
            return _tables.Contains(name);
        }

        public bool Contains(String name, String tableNamespace)
        {
            return _tables.Contains(name, tableNamespace);
        }

        public void CopyTo(FeatureDataTable[] array, Int32 arrayIndex)
        {
            _tables.CopyTo(array, arrayIndex);
        }

        public Int32 Count
        {
            get { return _tables.Count; }
        }

        public bool IsReadOnly
        {
            get { return _tables.IsReadOnly; }
        }

        public bool Remove(FeatureDataTable item)
        {
            bool contained = _tables.Contains(item.TableName);
            _tables.Remove(item);
            return contained;
        }

        public void Remove(String name)
        {
            _tables.Remove(name);
        }

        public void Remove(String name, String tableNamespace)
        {
            _tables.Remove(name, tableNamespace);
        }

        #endregion

        #region IEnumerable<FeatureDataTable> Members

        public IEnumerator<FeatureDataTable> GetEnumerator()
        {
            foreach (DataTable table in _tables)
            {
                yield return table as FeatureDataTable;
            }
        }

        #endregion

        #region IList<FeatureDataTable> Members

        void IList<FeatureDataTable>.Insert(Int32 index, FeatureDataTable item)
        {
            throw new NotSupportedException();
        }

        FeatureDataTable IList<FeatureDataTable>.this[Int32 index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private helper methods

        private void OnTablesChanged(CollectionChangeAction collectionChangeAction, object collection)
        {
            CollectionChangeEventHandler e = CollectionChanged;
            fireEvent(collectionChangeAction, collection, e);
        }

        private void OnTablesChanging(CollectionChangeAction collectionChangeAction, object collection)
        {
            CollectionChangeEventHandler e = CollectionChanging;
            fireEvent(collectionChangeAction, collection, e);
        }

        private void fireEvent(CollectionChangeAction collectionChangeAction, object collection,
                               CollectionChangeEventHandler e)
        {
            if (e != null)
            {
                CollectionChangeEventArgs args = new CollectionChangeEventArgs(collectionChangeAction, collection);
                e(this, args);
            }
        }

        private void tablesChanged(object sender, CollectionChangeEventArgs e)
        {
            OnTablesChanged(e.Action, e.Element);
        }

        private void tablesChanging(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                if (!(e.Element is FeatureDataTable))
                {
                    throw new InvalidOperationException("The table being added to the FeatureDataSet " +
                                                        "must be of type FeatureDataTable.");
                }
            }

            OnTablesChanging(e.Action, e.Element);
        }

        #endregion
    }
}