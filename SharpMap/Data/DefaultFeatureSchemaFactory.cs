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
    public class DefaultFeatureSchemaFactory : IFeaturesSchemaFactory
    {
        #region IFeaturesSchemaFactory Members

        public IFeaturesSchemaFactory New()
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchemaFactory New(string name, FeaturePropertyDataType type)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchemaFactory New(string name, FeaturePropertyDataType type, int size)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchemaFactory Add(string name, FeaturePropertyDataType type)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchemaFactory Add(string name, FeaturePropertyDataType type, int size)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchemaFactory SetId(int index)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchemaFactory SetGeometry(int index)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchema Done()
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchema New(IFeatureProperty attribute)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchema New(IEnumerable<IFeatureProperty> attributes)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchema Add(IFeatureProperty attribute)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchema Add(IEnumerable<IFeatureProperty> attributes)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchema SetId(IFeatureProperty attribute)
        {
            throw new NotImplementedException();
        }

        public IFeaturesSchema SetGeometry(IFeatureProperty attribute)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
