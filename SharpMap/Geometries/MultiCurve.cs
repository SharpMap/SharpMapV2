// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Geometries
{
	/// <summary>
	/// A MultiCurve is a one-dimensional GeometryCollection whose elements are Curves
	/// </summary>
	public abstract class MultiCurve<TCurve> : GeometryCollection<TCurve>
        where TCurve : Curve
	{
		/// <summary>
		/// Initializes an instance of a MultiLineString
		/// </summary>
		protected MultiCurve() { }

        protected MultiCurve(Int32 initialCapacity)
            : base(initialCapacity) { }

		/// <summary>
		/// The inherent dimension of this Geometry object, 
		/// which must be less than or equal to the coordinate dimension.
		/// </summary>
		public override Int32 Dimension
		{
			get { return 1; }
		}

		/// <summary>
		/// Returns true if this MultiCurve is closed.
		/// </summary>
		/// <remarks>
        /// A closed multi-curve is one where 
        /// <see cref="Curve.StartPoint"/>.Equals(<see cref="Curve.EndPoint"/>) 
        /// is true for each curve in the collection.
		/// </remarks>
		public Boolean IsClosed
        {
            get
            {
                foreach (TCurve curve in this)
                {
                    if(!curve.IsClosed)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

		/// <summary>
		/// The Length of this MultiCurve which is equal 
		/// to the sum of the lengths of the element Curves.
		/// </summary>
		public Double Length 
        {
            get
            {
                Double length = 0.0;

                foreach (TCurve curve in this)
                {
                    length += curve.Length;
                }

                return length;
            }
        }
	}
}