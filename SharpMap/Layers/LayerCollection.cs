// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Collections.Generic;
using System.ComponentModel;
using GeoAPI.Algorithms;

namespace SharpMap.Layers
{
    /// <summary>
    /// Represents an ordered collection of layers of geospatial features
    /// which are composed into a map.
    /// </summary>
    public class LayerCollection : BindingList<ILayer>, ITypedList
    {
        private readonly Map _map;
        private Boolean? _sortedAscending;
        private readonly Object _collectionChangeSync = new Object();
        private PropertyDescriptor _sortProperty;
        private static readonly PropertyDescriptorCollection _layerProperties;
        internal readonly Object LayersChangeSync = new Object();

        static LayerCollection()
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(ILayer));
            PropertyDescriptor[] propsArray = new PropertyDescriptor[props.Count];
            props.CopyTo(propsArray, 0);

            _layerProperties = new PropertyDescriptorCollection(propsArray, true);
        }

        internal LayerCollection(Map map)
        {
            _map = map;
            base.AllowNew = false;
        }

        internal void AddRange(IEnumerable<ILayer> layers)
        {
            RaiseListChangedEvents = false;

            foreach (ILayer layer in layers)
            {
                Add(layer);
            }

            RaiseListChangedEvents = true;
            ResetBindings();
        }

        internal Boolean Exists(Predicate<ILayer> predicate)
        {
            foreach (ILayer layer in this)
            {
                if (predicate(layer))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a layer by its name, or <see langword="null"/> if the layer isn't found.
        /// </summary>
        /// <remarks>
        /// Performs culture-specific, case-insensitive search.
        /// </remarks>
        /// <param name="layerName">Name of layer.</param>
        /// <returns>
        /// Layer with <see cref="ILayer.LayerName"/> of <paramref name="layerName"/>.
        /// </returns>
        public ILayer this[String layerName]
        {
            get { return _map.GetLayerByName(layerName); }
        }

        #region AddNew support

        protected override Object AddNewCore()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Adding new layers through LayerCollection is not supported. Always gets
        /// <see langword="false"/> and throws an exception if set.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if set.</exception>
        public new Boolean AllowNew
        {
            get { return false; }
            set { throw new NotSupportedException(); }
        }
#if DEBUG
        // ReSharper disable RedundantOverridenMember
        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        public override void CancelNew(Int32 itemIndex)
        {
            base.CancelNew(itemIndex);
        }

        public override void EndNew(Int32 itemIndex)
        {
            base.EndNew(itemIndex);
        }
        // ReSharper restore RedundantOverridenMember
#endif
        #endregion

        #region Sorting support

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            lock (_collectionChangeSync)
            {
                try
                {
                    RaiseListChangedEvents = false;

                    if (prop.Name == "LayerName")
                    {
                        Int32 sortDirValue = (direction == ListSortDirection.Ascending
                                                  ? 1
                                                  : -1);
                        
                        const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;

                        Comparison<ILayer> comparison = delegate(ILayer lhs, ILayer rhs)
                                                        {
                                                            return sortDirValue *
                                                                   String.Compare(lhs.LayerName,
                                                                                  rhs.LayerName,
                                                                                  ignoreCase);
                                                        };

                        QuickSort.Sort(this, comparison);
                    }

                    _sortedAscending = (direction == ListSortDirection.Ascending);
                    _sortProperty = prop;
                }
                finally
                {
                    RaiseListChangedEvents = true;
                    ResetBindings();
                }
            }
        }

        protected override void RemoveSortCore()
        {
            _sortedAscending = null;
        }

        protected override Boolean SupportsSortingCore
        {
            get { return true; }
        }

        protected override Boolean IsSortedCore
        {
            get { return _sortedAscending.HasValue; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                if (!_sortedAscending.HasValue)
                {
                    throw new InvalidOperationException("List is not sorted.");
                }

                return ((Boolean)_sortedAscending)
                           ? ListSortDirection.Ascending
                           : ListSortDirection.Descending;
            }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }

        #endregion

        #region Searching support

        protected override Int32 FindCore(PropertyDescriptor prop, Object key)
        {
            switch (prop.Name)
            {
                case "LayerName":
                    String layerName = key as String;

                    if (String.IsNullOrEmpty(layerName))
                    {
                        throw new ArgumentException("Layer name must be a non-null, non-empty String.");
                    }

                    foreach (ILayer layer in this)
                    {
                        if (String.Compare(layerName,
                                           layer.LayerName,
                                           StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            return IndexOf(layer);
                        }
                    }

                    return -1;
                default:
                    throw new NotSupportedException("Only sorting on the layer name " +
                                                    "is currently supported.");
            }
        }

        protected override Boolean SupportsSearchingCore
        {
            get { return true; }
        }

        #endregion
        
#if DEBUG
        // ReSharper disable RedundantOverridenMember
        protected override void ClearItems()
        {
            base.ClearItems();
        }

        protected override void InsertItem(Int32 index, ILayer item)
        {
            base.InsertItem(index, item);
        }
        // ReSharper restore RedundantOverridenMember
#endif

        protected override void RemoveItem(Int32 index)
        {
            // This defines the missing "OnDeleting" functionality:
            // having a ListChangedEventArgs.NewIndex == -1 and
            // the index of the item pending removal to be 
            // ListChangedEventArgs.OldIndex.
            ListChangedEventArgs args
                = new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, index);

            OnListChanged(args);

            base.RemoveItem(index);
        }
        
#if DEBUG
        // ReSharper disable RedundantOverridenMember
        protected override void SetItem(Int32 index, ILayer item)
        {
            base.SetItem(index, item);
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }
        // ReSharper restore RedundantOverridenMember
#endif

        protected override Boolean SupportsChangeNotificationCore
        {
            get { return true; }
        }

        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null)
            {
                throw new NotSupportedException("Child lists not supported in LayersCollection.");
            }

            return _layerProperties;
        }

        /// <summary>
        /// Returns the name of the list.
        /// </summary>
        /// <param name="listAccessors">
        /// An array of <see cref="PropertyDescriptor"/> objects, 
        /// for which the list name is returned. This can be <see langword="null"/>.
        /// </param>
        /// <returns>The name of the list.</returns>
        /// <remarks>
        /// From the MSDN docs: This method is used only in the design-time framework 
        /// and by the obsolete DataGrid control.
        /// </remarks>
        public String GetListName(PropertyDescriptor[] listAccessors)
        {
            return "LayerCollection";
        }

        #endregion
    }
}