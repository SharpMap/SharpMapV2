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
using System.Collections;
using SharpMap.Data;
using GeoAPI.Geometries;
using SharpMap.Expressions;
using SharpMap.Styles;

namespace SharpMap.Layers
{
	/// <summary>
	/// A map layer of raster data.
	/// </summary>
	/// <example>
	/// Adding a <see cref="RasterLayer"/> to a map:
	/// </example>
	public class RasterLayer : Layer, IRasterLayer
	{
		public RasterLayer(IProvider dataSource)
			: base(dataSource)
		{
		}

		public override Object Clone()
		{
			throw new NotImplementedException();
        }

        protected override IStyle CreateStyle()
        {
            return new RasterStyle();
        }

	    protected override void ProcessLoadResults(object results)
	    {
	        throw new NotImplementedException();
	    }

	    public override IEnumerable Select(Expression query)
	    {
	        throw new NotImplementedException();
	    }

	    #region IRasterLayer Members

	    public new IRasterProvider DataSource
        {
            get { throw new NotImplementedException(); }
        }

	    #endregion
	}
}