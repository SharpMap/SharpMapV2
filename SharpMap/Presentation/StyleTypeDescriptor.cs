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
using SharpMap.Styles;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Provides a description of a layer style.
    /// </summary>
    public class StyleTypeDescriptor : ICustomTypeDescriptor
    {
        private readonly Type _styleType;

        public StyleTypeDescriptor(Type styleType)
        {
            if (styleType.GetInterface(typeof(IStyle).Name, false) == null)
            {
                throw new ArgumentException(
                    "Parameter must be a type which implements SharpMap.Styles.IStyle.",
                    "styleType");
            }

            _styleType = styleType;
        }

        public Type StyleType
        {
            get { return _styleType; }
        }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        public String GetClassName()
        {
            return typeof (VectorStyle).Name;
        }

        public String GetComponentName()
        {
            return null;
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public Object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        public EventDescriptorCollection GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (attributes.Length > 0)
            {
                return PropertyDescriptorCollection.Empty;
            }
            else
            {
                return GetProperties();
            }
        }

        public PropertyDescriptorCollection GetProperties()
        {
            throw new NotImplementedException();
        }

        public Object GetPropertyOwner(PropertyDescriptor pd)
        {
            return null;
        }

        #endregion
    }
}