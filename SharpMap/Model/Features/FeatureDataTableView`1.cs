using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Presentation;

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
            throw new Exception("The method or operation is not implemented.");
        }

        public int Find(System.ComponentModel.PropertyDescriptor property, FeatureDataRow<TOid> key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IList<FeatureDataRow<TOid>> Members

        public int IndexOf(FeatureDataRow<TOid> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, FeatureDataRow<TOid> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public FeatureDataRow<TOid> this[int index]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<FeatureDataRow<TOid>> Members

        public void Add(FeatureDataRow<TOid> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(FeatureDataRow<TOid> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CopyTo(FeatureDataRow<TOid>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Remove(FeatureDataRow<TOid> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<FeatureDataRow<TOid>> Members

        public IEnumerator<FeatureDataRow<TOid>> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IBindingListView Members

        public void ApplySort(System.ComponentModel.ListSortDescriptionCollection sorts)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string Filter
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void RemoveFilter()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.ComponentModel.ListSortDescriptionCollection SortDescriptions
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool SupportsAdvancedSorting
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool SupportsFiltering
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IBindingList Members

        public void AddIndex(System.ComponentModel.PropertyDescriptor property)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object System.ComponentModel.IBindingList.AddNew()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool AllowEdit
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool AllowNew
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool AllowRemove
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Find(System.ComponentModel.PropertyDescriptor property, object key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsSorted
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public event System.ComponentModel.ListChangedEventHandler ListChanged;

        public void RemoveIndex(System.ComponentModel.PropertyDescriptor property)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveSort()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.ComponentModel.ListSortDirection SortDirection
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public System.ComponentModel.PropertyDescriptor SortProperty
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool SupportsChangeNotification
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool SupportsSearching
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool SupportsSorting
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IndexOf(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsFixedSize
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Remove(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsSynchronized
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region ITypedList Members

        public System.ComponentModel.PropertyDescriptorCollection GetItemProperties(System.ComponentModel.PropertyDescriptor[] listAccessors)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetListName(System.ComponentModel.PropertyDescriptor[] listAccessors)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
