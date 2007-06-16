using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SharpMap.Presentation
{
    public interface IBindingListView<TItem> : IList<TItem>, IBindingListView, ITypedList
    {
        /// <summary>
        /// Adds a new item to the list.
        /// </summary>
        /// <returns>The item added to the list.</returns>
        /// <exception cref="System.NotSupportedException">System.ComponentModel.IBindingList.AllowNew is false.</exception>
        new TItem AddNew();

        /// <summary>
        /// Returns the index of the row that has the given <see cref="System.ComponentModel.PropertyDescriptor"/>.
        /// </summary>
        /// <param name="property">The <see cref="System.ComponentModel.PropertyDescriptor"/> to search on.</param>
        /// <param name="key">The value of the property parameter to search for.</param>
        /// <returns>The index of the row that has the given <see cref="System.ComponentModel.PropertyDescriptor"/>.</returns>
        /// <exception cref="System.NotSupportedException"><see cref="System.ComponentModel.IBindingList.SupportsSearching"/> is false.</exception>
        int Find(PropertyDescriptor property, TItem key);
    }
}
