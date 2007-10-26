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
using System.Collections;
using System.Collections.Generic;

namespace SharpMap.Geometries
{
    /// <summary>
    /// A GeometryCollection is a geometry that is a 
    /// collection of 1 or more geometries.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All the elements in a GeometryCollection must be in the same 
    /// spatial reference system. This is also the spatial
    /// reference for the GeometryCollection.
    /// </para>
    /// <para>
    /// GeometryCollection places no other constraints on its elements. 
    /// Subclasses of GeometryCollection may
    /// restrict membership based on dimension or
    /// other topological constraints, such as degree of spatial overlap 
    /// between elements.
    /// </para>
    /// </remarks>
    public class GeometryCollection<TGeometry> : Geometry, IGeometryCollection,
        IEquatable<GeometryCollection<TGeometry>>, IEnumerable<TGeometry>
        where TGeometry : Geometry
    {
        private readonly List<TGeometry> _geometries;

        /// <summary>
        /// Initializes a new GeometryCollection
        /// </summary>
        public GeometryCollection()
            : this(16)
        {
        }

        /// <summary>
        /// Initializes a new GeometryCollection
        /// </summary>
        public GeometryCollection(Int32 initialCapacity)
        {
            _geometries = new List<TGeometry>(initialCapacity);
        }

        /// <summary>
        /// Gets the number of geometries in the collection.
        /// </summary>
        public virtual Int32 NumGeometries
        {
            get { return _geometries.Count; }
        }

        /// <summary>
        /// Returns an indexed geometry in the collection.
        /// </summary>
        /// <param name="index">Geometry index.</param>
        /// <returns>Geometry at given index.</returns>
        public virtual Geometry Geometry(Int32 index)
        {
            return _geometries[index];
        }

        /// <summary>
        /// Returns an indexed geometry in the collection.
        /// </summary>
        /// <param name="index">Geometry index.</param>
        /// <returns>Geometry</returns>
        public virtual TGeometry this[Int32 index]
        {
            get { return _geometries[index]; }
        }

        /// <summary>
        /// Determines if all the geometries are 
        /// empty or the collection is empty.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the GeometryCollection is 
        /// empty or contains only null or empty geometries.
        /// </returns>
        public override Boolean IsEmpty()
        {
            if (_geometries == null)
            {
                return true;
            }

            foreach (TGeometry g in Collection)
            {
                if (g != null && !g.IsEmpty())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the collection of geometries.
        /// </summary>
        public virtual IList<TGeometry> Collection
        {
            get { return _geometries; }
        }

        /// <summary>
        /// The inherent dimension of this Geometry object, 
        /// which must be less than or equal
        /// to the coordinate dimension.
        /// </summary>
        public override Int32 Dimension
        {
            get
            {
                Int32 dim = 0;
                _geometries.ForEach(delegate(TGeometry g) { dim = Math.Max(dim, g.Dimension); });
                return dim;
            }
        }

        /// <summary>
        /// The minimum bounding box for this Geometry, returned as a BoundingBox.
        /// </summary>
        /// <returns></returns>
        public override BoundingBox GetBoundingBox()
        {
            BoundingBox b = BoundingBox.Empty;

            if (NumGeometries == 0)
            {
                return b;
            }

            _geometries.ForEach(delegate(TGeometry g) { b.ExpandToInclude(g.GetBoundingBox()); });

            return b;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> 
        /// is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override Int32 GetHashCode()
        {
            Int32 hash = 0;
            _geometries.ForEach(delegate(TGeometry g) { hash ^= g.GetHashCode(); });
            return hash;
        }

        #region IEquatable<GeometryCollection<TGeometry>> Members

        /// <summary>
        /// Determines whether the GeometryCollection{TGeometry} is spatially equal 
        /// to the GeometryCollection{TGeometry} <paramref name="other"/>.
        /// </summary>
        /// <param name="other">
        /// Another instance of GeometryCollection{TGeometry} to compare to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the GeometryCollections are equal.
        /// </returns>
        public Boolean Equals(GeometryCollection<TGeometry> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (other.Collection.Count != Collection.Count)
            {
                return false;
            }

            unchecked
            {
                for (Int32 i = 0; i < other.Collection.Count; i++)
                {
                    if (!other.Collection[i].Equals(Collection[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Returns <see langword="true"/> if this Geometry has no anomalous geometric points, 
        /// such as self intersection or self tangency. The description of each 
        /// instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the geometry is simple</returns>
        public override Boolean IsSimple()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the closure of the combinatorial boundary of this Geometry. 
        /// The combinatorial boundary is defined as described in section 3.12.3.2 of [1]. 
        /// Because the result of this function is a closure, and hence topologically closed, 
        /// the resulting boundary can be represented using representational geometry primitives.
        /// </summary>
        /// <returns>Closure of the combinatorial boundary of this Geometry</returns>
        public override Geometry Boundary()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the shortest distance between any two points in the two geometries
        /// as calculated in the spatial reference system of this Geometry.
        /// </summary>
        /// <param name="geom">Geometry to calculate distance to</param>
        /// <returns>Shortest distance between any two points in the two geometries</returns>
        public override Double Distance(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents all points whose distance from this Geometry
        /// is less than or equal to distance. Calculations are in the Spatial Reference
        /// System of this Geometry.
        /// </summary>
        /// <param name="d">Buffer distance</param>
        /// <returns>Buffer around geometry</returns>
        public override Geometry Buffer(Double d)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Geometry—Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        /// <returns>The convex hull</returns>
        public override Geometry ConvexHull()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set intersection of this Geometry
        /// with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to intersect with</param>
        /// <returns>
        /// Returns a geometry that represents the point set 
        /// intersection of this Geometry with another Geometry.
        /// </returns>
        public override Geometry Intersection(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set 
        /// union of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geometry">Geometry to union with</param>
        /// <returns>Unioned geometry</returns>
        public override Geometry Union(Geometry geometry)
        {
#warning broken implementation awaiting for replacement with NTS in Beta 2
            if (IsEmpty())
            {
                return geometry;
            }

            if (geometry.IsEmpty())
            {
                return this;
            }

            if (geometry is TGeometry)
            {
                Collection.Add(geometry as TGeometry);
                return this;
            }
            else
            {
                GeometryCollection union = new GeometryCollection();
                union.Collection.Add(this);
                union.Collection.Add(geometry);
                return union;
            }
        }

        /// <summary>
        /// Returns a geometry that represents the point set 
        /// difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override Geometry Difference(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set symmetric 
        /// difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override Geometry SymDifference(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Point> GetVertices()
        {
            foreach (TGeometry geometry in _geometries)
            {
                foreach (Point point in geometry.GetVertices())
                {
                    yield return point;
                }
            }
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a deep copy of the GeometryCollection.
        /// </summary>
        /// <returns>A copy of the GeometryCollection instance.</returns>
        public override Geometry Clone()
        {
            GeometryCollection geoms = new GeometryCollection();

            foreach (TGeometry geometry in Collection)
            {
                geoms.Collection.Add(geometry.Clone());
            }

            return geoms;
        }

        #endregion

        #region IEnumerable<TGeometry> Members

        /// <summary>
        /// Gets an enumerator for enumerating the geometries in the GeometryCollection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TGeometry> GetEnumerator()
        {
            foreach (TGeometry g in Collection)
            {
                yield return g;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an enumerator for enumerating the geometries in the GeometryCollection
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
