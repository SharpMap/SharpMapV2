using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Presentation;
using System.ComponentModel;

namespace SharpMap
{
    public class FeatureDataTableView<TOid> : IBindingListView<FeatureDataRow<TOid>>
    {
        public FeatureDataTableView(FeatureDataTable featureTable)
        {
        }

        public FeatureDataTableView(FeatureDataTable featureTable, string sort)
        {
        }

        public FeatureDataTableView(FeatureDataTable featureTable, string sort, string filter)
        {
        }

        public FeatureDataTableView(FeatureDataTable featureTable, string sort, string filter, System.Data.DataViewRowState state)
        {
        }

        #region IBindingListView<FeatureDataRow<TOid>> Members

        public FeatureDataRow<TOid> AddNew()
        {
            throw new NotImplementedException();
        }

        public int Find(System.ComponentModel.PropertyDescriptor property, FeatureDataRow<TOid> key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IList<FeatureDataRow<TOid>> Members

        public int IndexOf(FeatureDataRow<TOid> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, FeatureDataRow<TOid> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public FeatureDataRow<TOid> this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<FeatureDataRow<TOid>> Members

        public void Add(FeatureDataRow<TOid> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(FeatureDataRow<TOid> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(FeatureDataRow<TOid>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(FeatureDataRow<TOid> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<FeatureDataRow<TOid>> Members

        public IEnumerator<FeatureDataRow<TOid>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBindingListView Members

        public void ApplySort(System.ComponentModel.ListSortDescriptionCollection sorts)
        {
            throw new NotImplementedException();
        }

        public string Filter
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void RemoveFilter()
        {
            throw new NotImplementedException();
        }

        public System.ComponentModel.ListSortDescriptionCollection SortDescriptions
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsAdvancedSorting
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsFiltering
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IBindingList Members

        public void AddIndex(System.ComponentModel.PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        object System.ComponentModel.IBindingList.AddNew()
        {
            throw new NotImplementedException();
        }

        public bool AllowEdit
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowNew
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowRemove
        {
            get { throw new NotImplementedException(); }
        }

        public void ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public int Find(System.ComponentModel.PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public bool IsSorted
        {
            get { throw new NotImplementedException(); }
        }

        public event ListChangedEventHandler ListChanged;

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }

        public System.ComponentModel.ListSortDirection SortDirection
        {
            get { throw new NotImplementedException(); }
        }

        public System.ComponentModel.PropertyDescriptor SortProperty
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsChangeNotification
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsSearching
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsSorting
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection Members

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

        #region ITypedList Members

        public System.ComponentModel.PropertyDescriptorCollection GetItemProperties(System.ComponentModel.PropertyDescriptor[] listAccessors)
        {
            throw new NotImplementedException();
        }

        public string GetListName(System.ComponentModel.PropertyDescriptor[] listAccessors)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
