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
using System.Diagnostics;
using GeoAPI.Geometries;

namespace SharpMap.SimpleGeometries
{
	/// <summary>
	/// A MultiPoint is a 0 dimensional geometric collection. 
	/// The elements of a MultiPoint are restricted to Points. 
	/// The points are not connected or ordered.
	/// </summary>
    public class MultiPoint : GeometryCollection<Point>, IMultiPoint
	{
        /// <summary>
        /// Initializes a new MultiPoint collection
        /// </summary>
        internal MultiPoint() { }

		internal MultiPoint(Int32 initialCapacity)
            : base(initialCapacity) { }

		/// <summary>
		///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
		/// </summary>
		public override Dimensions Dimension
		{
            get { return Dimensions.Point; }
		}

		/// <summary>
		/// Creates a copy of this geometry.
		/// </summary>
		/// <returns>Copy of the MultiPoint.</returns>
		public override Geometry Clone()
		{
            MultiPoint multiPoint = FactoryInternal.CreateMultiPoint() as MultiPoint;
		    Debug.Assert(multiPoint != null);

    			foreach (Point p in (IEnumerable<IPoint>)this)
			{
				multiPoint.Add(p.Clone() as Point);
			}

			return multiPoint;
		}

        #region IMultiPoint Members

        public new IPoint this[Int32 index]
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


        IGeometry IList<IGeometry>.this[Int32 index]
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

        #region IEnumerable<IPoint> Members

        IEnumerator<IPoint> IEnumerable<IPoint>.GetEnumerator()
        {
            IEnumerator<Point> enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        #endregion
    }
}