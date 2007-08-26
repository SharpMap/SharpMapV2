// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
