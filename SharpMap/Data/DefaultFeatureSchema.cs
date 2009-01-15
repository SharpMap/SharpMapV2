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
using System.Collections.Generic;

namespace SharpMap.Data
{
    public class DefaultFeatureSchema : IFeaturesSchema
    {
        private readonly Dictionary<String, Int32> _propertyNameIndex = new Dictionary<string, int>();
        private readonly List<IFeatureProperty> _properties = new List<IFeatureProperty>();
        private IFeatureProperty _id;
        private IFeatureProperty _geometry;
        private DefaultFeatureCache _configuredCache;

        public void AddProperty(IFeatureProperty property)
        {
            if (_propertyNameIndex.ContainsKey(property.Name))
            {
                throw new ArgumentException("Schema already contains attribute with name: " + property.Name);
            }

            _properties.Add(property);
            _propertyNameIndex[property.Name] = _properties.Count - 1;
        }

        public void SetId(Int32 index)
        {
            _id = _properties[index];
        }

        public void SetGeometry(Int32 index)
        {
            _geometry = _properties[index];
        }

        #region IFeaturesSchema Members

        public int PropertyCount
        {
            get { return _properties.Count; }
        }

        public void SetCacheSchema(IFeaturesCache cache)
        {
            if (_configuredCache == null)
            {
                initConfiguredCache();
            }

            cache.MergeSchemaFrom(_configuredCache);
        }

        public IList<IFeatureProperty> Properties
        {
            get { return _properties.AsReadOnly(); }
        }

        public IFeatureProperty IdProperty
        {
            get { return _id; }
            internal set
            {
                _id = value;
            }
        }

        public bool HasIdProperty
        {
            get { return _id != null; }
        }

        public IFeatureProperty GeometryProperty
        {
            get { return _geometry; }
        }

        public bool HasGeometryProperty
        {
            get { return _geometry != null; }
        }

        public IFeatureProperty this[int index]
        {
            get { return _properties[index]; }
        }

        public IFeatureProperty this[string name]
        {
            get
            {
                Int32 index;

                if (_propertyNameIndex.TryGetValue(name, out index))
                {
                    return _properties[index];
                }

                return null;
            }
        }

        public int GetIndex(IFeatureProperty property)
        {
            return _properties.IndexOf(property);
        }

        public int GetIndex(string name)
        {
            Int32 index;

            if (_propertyNameIndex.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;
        }

        #endregion

        private void initConfiguredCache()
        {
            _configuredCache = new DefaultFeatureCache(this);
        }

        #region Implementation of IEquatable<IFeaturesSchema>

        public bool Equals(IFeaturesSchema other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.PropertyCount != PropertyCount)
            {
                return false;
            }

            for (int i = 0; i < PropertyCount; i++)
            {
                if (!this[i].Equals(other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
