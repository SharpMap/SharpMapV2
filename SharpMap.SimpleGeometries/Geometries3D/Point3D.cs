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
using GeoAPI.Geometries;

namespace SharpMap.SimpleGeometries.Geometries3D
{
	/// <summary>
	/// A Point3D is a 0-dimensional geometry and represents a single location 
	/// in 3D coordinate space. A Point3D has a x coordinate value, 
	/// a y-coordinate value and a z-coordinate value. 
	/// The boundary of a Point3D is the empty set.
	/// </summary>
	[Serializable]
	public class Point3D : Point, IPoint3D
	{
		private static readonly Point3D _empty = new Point3D();
		private Double _z;

		/// <summary>
		/// Initializes a new Point
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="z">Z coordinate</param>
        internal protected Point3D(GeometryFactory factory, Double x, Double y, Double z)
            : base(factory, x, y)
		{
			_z = z;
		}

		/// <summary>
		/// Initializes a new Point
		/// </summary>
		/// <param name="p">2D Point</param>
		/// <param name="z">Z coordinate</param>
		public Point3D(Point p, Double z)
            : base(p.FactoryInternal, p.X, p.Y)
		{
			_z = z;
		}

		/// <summary>
		/// Initializes a new Point at (0,0)
		/// </summary>
        internal protected Point3D()
		{
			SetEmpty();
		}

		/// <summary>
		/// Gets or sets the Z coordinate of the point
		/// </summary>
		public Double Z
		{
			get
			{
				if (!IsEmpty)
					return _z;
				else
					throw new InvalidOperationException("Point is empty");
			}
			set
			{
				_z = value;
				SetNotEmpty();
			}
		}

		/// <summary>
		/// Returns part of coordinate. Index 0 = X, Index 1 = Y, , Index 2 = Z
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override Double this[UInt32 index]
		{
			get
			{
                if (index == 2)
                {
                    if (IsEmpty)
                    {
                        throw new InvalidOperationException("Point is empty");
                    }

                    return Z;
                }
                else
                {
                    return base[index];
                }
			}
            //set
            //{
            //    if (index == 2)
            //    {
            //        Z = value;
            //        SetNotEmpty();
            //    }
            //    else
            //        base[index] = value;
            //}
		}

		#region Operators

        ///// <summary>
        ///// Vector + Vector
        ///// </summary>
        ///// <param name="v1">Vector</param>
        ///// <param name="v2">Vector</param>
        ///// <returns></returns>
        //public static Point3D operator +(Point3D v1, Point3D v2)
        //{
        //    return new Point3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        //}


        ///// <summary>
        ///// Vector - Vector
        ///// </summary>
        ///// <param name="v1">Vector</param>
        ///// <param name="v2">Vector</param>
        ///// <returns>Cross product</returns>
        //public static Point3D operator -(Point3D v1, Point3D v2)
        //{
        //    return new Point3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        //}

        ///// <summary>
        ///// Vector * Scalar
        ///// </summary>
        ///// <param name="m">Vector</param>
        ///// <param name="d">Scalar (Double)</param>
        ///// <returns></returns>
        //public static Point3D operator *(Point3D m, Double d)
        //{
        //    return new Point3D(m.X*d, m.Y*d, m.Z*d);
        //}

		#endregion

		#region "Inherited methods from abstract class Geometry"

		/// <summary>
		/// Checks whether this instance is spatially equal to the Point 'o'
		/// </summary>
		/// <param name="p">Point to compare to</param>
		/// <returns></returns>
		public Boolean Equals(Point3D p)
		{
			return base.Equals(p) && p.Z == _z;
		}

		/// <summary>
		/// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
		/// in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
		public override Int32 GetHashCode()
		{
			return base.GetHashCode() ^ _z.GetHashCode();
		}

		/// <summary>
		/// Returns the distance between this geometry instance and another geometry, as
		/// measured in the spatial reference system of this instance.
		/// </summary>
		/// <param name="geom"></param>
		/// <returns></returns>
		public Double Distance(Geometry geom)
		{
			if (geom is Point3D)
			{
				Point3D p = geom as Point3D;
				return Math.Sqrt(Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2) + Math.Pow(Z - p.Z, 2));
			}
			else
				return base.Distance(geom);
		}

		#endregion

		/// <summary>
		/// This method must be overridden using 'public new [derived_data_type] Clone()'
		/// </summary>
		/// <returns>Clone</returns>
		public new Point3D Clone()
		{
			return new Point3D(FactoryInternal, X, Y, Z);
		}

		#region IEqualityComparer<Point3D> Members

		/// <summary>
		/// Checks whether the two points are spatially equal
		/// </summary>
		/// <param name="p1">Point 1</param>
		/// <param name="p2">Point 2</param>
		/// <returns>true if the points a spatially equal</returns>
		public Boolean Equals(Point3D p1, Point3D p2)
		{
			return (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z);
		}

		#endregion

		#region IComparable<Point> Members

		/// <summary>
		/// Comparator used for ordering point first by ascending X, then by ascending Y and then by ascending Z.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual Int32 CompareTo(Point3D other)
		{
			if (X < other.X || X == other.X && Y < other.Y || X == other.X && Y == other.Y && Z < other.Z)
				return -1;
			else if (X > other.X || X == other.X && Y > other.Y || X == other.X && Y == other.Y && Z > other.Z)
				return 1;
			else // (this.X == other.X && this.Y == other.Y && this.Z == other.Z)
				return 0;
		}

		#endregion

		protected override void SetEmpty()
		{
			base.SetEmpty();
			_z = 0;
		}
	}
}