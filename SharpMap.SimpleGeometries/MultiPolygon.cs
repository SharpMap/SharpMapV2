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
using GeoAPI.Geometries;

namespace SharpMap.SimpleGeometries
{
	/// <summary>
	/// A MultiPolygon is a MultiSurface whose elements are Polygons.
	/// </summary>
	[Serializable]
	public class MultiPolygon : MultiSurface<Polygon>, IMultiPolygon
	{
		/// <summary>
		/// Instantiates a MultiPolygon
		/// </summary>
		public MultiPolygon() { }

		public MultiPolygon(Int32 initialCapacity) :
            base(initialCapacity) { }

        ///// <summary>
        ///// Collection of polygons in the multipolygon
        ///// </summary>
        //public IList<Polygon> Polygons
        //{
        //    get { return Collection; }
        //}

		/// <summary>
		/// The mathematical centroid for the surfaces as a Point.
		/// The result is not guaranteed to be on any of the surfaces.
		/// </summary>
		public override Point Centroid
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// A point guaranteed to be on this Surface.
		/// </summary>
		public override Point PointOnSurface
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Creates a copy of this geometry.
		/// </summary>
		/// <returns>Copy of the MultiPolygon.</returns>
		public override Geometry Clone()
		{
			MultiPolygon multiPolygon = new MultiPolygon();

			foreach (Polygon poly in this)
			{
				multiPolygon.Add(poly.Clone() as Polygon);
			}

			return multiPolygon;
		}

        #region IMultiPolygon Members

        public new IPolygon this[Int32 index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IList<IGeometry> Members


        IGeometry System.Collections.Generic.IList<IGeometry>.this[Int32 index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IEnumerable<IGeometry> Members

        IEnumerator<IGeometry> IEnumerable<IGeometry>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<IPolygon> Members

        public new IEnumerator<IPolygon> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}