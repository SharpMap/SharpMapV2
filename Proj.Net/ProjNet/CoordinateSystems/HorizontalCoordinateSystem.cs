// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using GeoAPI.CoordinateSystems;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems
{
	/// <summary>
	/// A 2D coordinate system suitable for positions on the Earth's surface.
	/// </summary>
    public abstract class HorizontalCoordinateSystem<TCoordinate> : CoordinateSystem<TCoordinate>, IHorizontalCoordinateSystem<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>, IConvertible
	{
		private IHorizontalDatum _horizontalDatum;

		/// <summary>
		/// Creates an instance of HorizontalCoordinateSystem
		/// </summary>
		/// <param name="datum">Horizontal datum</param>
		/// <param name="axisInfo">Axis information</param>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="code">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal HorizontalCoordinateSystem(IHorizontalDatum datum, IEnumerable<AxisInfo> axisInfo, 
			String name, String authority, long code, String alias,
			String remarks, String abbreviation)
			: base(name, authority, code, alias, abbreviation, remarks)
		{
			_horizontalDatum = datum;

			AxisInfo = new List<AxisInfo>(axisInfo);

			if (AxisInfo.Count != 2)
			{
			    throw new ArgumentException("Axis info should contain two axes for horizontal coordinate systems");
			}
		}

		#region IHorizontalCoordinateSystem Members

		/// <summary>
		/// Gets or sets the HorizontalDatum.
		/// </summary>
		public IHorizontalDatum HorizontalDatum
		{
			get { return _horizontalDatum; }
			set { _horizontalDatum = value; }
		}
		
		#endregion
	}
}
