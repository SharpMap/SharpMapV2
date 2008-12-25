// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.ComponentModel;

namespace SharpMap.Data
{
    public interface IHasDynamicProperties : INotifyPropertyChanged, IDataObject
    {
        /// <summary>
        /// Adds a dynamic property.
        /// </summary>
        /// <param name="property">The property to add.</param>
        /// <returns>The index of the added property.</returns>
        Int32 AddProperty(PropertyDescriptor property);

        /// <summary>
        /// Adds a dynamic property and sets its value.
        /// </summary>
        /// <param name="property">The property to add.</param>
        /// <param name="value">The value of the property.</param>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <returns>The index of the added property.</returns>
        Int32 AddProperty<TValue>(PropertyDescriptor property, TValue value);

        /// <summary>
        /// Gets the value of the property indicated by the given <see cref="PropertyDescriptor"/>.
        /// </summary>
        /// <param name="property">
        /// The property to get the value of, or <see langword="null"/> if the layer doesn't
        /// have <paramref name="property"/>.
        /// </param>
        /// <returns>
        /// The value of <paramref name="property"/> or <see langword="null"/> if the layer doesn't
        /// have the property.
        /// </returns>
        Object GetPropertyValue(PropertyDescriptor property);

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="property">The property to get the value of.</param>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <returns>The index of the added property.</returns>
        TValue GetPropertyValue<TValue>(PropertyDescriptor property);

        /// <summary>
        /// Returns a value indicating whether the instance has the given <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The property to test for.</param>
        /// <returns>
        /// <see langword="true"/> if the object has the property has the property, 
        /// <see langword="false"/> if otherwise.
        /// </returns>
        Boolean HasProperty(PropertyDescriptor property);

        /// <summary>
        /// Sets the value of the property indicated by the given <see cref="PropertyDescriptor"/>,
        /// adding it if it doesn't exist.
        /// </summary>
        /// <param name="property">
        /// The property to set the value of.
        /// </param>
        /// <param name="value">
        /// The value to set <paramref name="property"/> to.
        /// </param>
        void SetPropertyValue(PropertyDescriptor property, Object value);

        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="property">The property to set the value of.</param>
        /// <param name="value">The value to set the property to.</param>
        void SetPropertyValue<TValue>(PropertyDescriptor property, TValue value);
    }
}
