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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GeoAPI.Geometries;
using GeoAPI.Utilities;
using NPack;
using NPack.Interfaces;

namespace SharpMap.SimpleGeometries
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
        IEquatable<GeometryCollection<TGeometry>>
        where TGeometry : Geometry
    {
        private readonly List<TGeometry> _geometries;

        /// <summary>
        /// Initializes a new GeometryCollection
        /// </summary>
        internal protected GeometryCollection()
            : this(16)
        {
        }

        /// <summary>
        /// Initializes a new GeometryCollection
        /// </summary>
        internal protected GeometryCollection(Int32 initialCapacity)
        {
            _geometries = new List<TGeometry>(initialCapacity);
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

        public override IPoint Centroid
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Determines if all the geometries are 
        /// empty or the collection is empty.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the GeometryCollection is 
        /// empty or contains only null or empty geometries.
        /// </returns>
        public override Boolean IsEmpty
        {
            get
            {
                if (_geometries == null)
                {
                    return true;
                }

                foreach (TGeometry g in this)
                {
                    if (g != null && !g.IsEmpty)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// The inherent dimension of this Geometry object, 
        /// which must be less than or equal
        /// to the coordinate dimension.
        /// </summary>
        public override Dimensions Dimension
        {
            get
            {
                Int32 dim = 0;
                _geometries.ForEach(delegate(TGeometry g) { dim = Math.Max(dim, (Int32)g.Dimension); });
                return (Dimensions)dim;
            }
        }

        public override Dimensions BoundaryDimension
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// The minimum bounding box for this Geometry, returned as a BoundingBox.
        /// </summary>
        /// <returns></returns>
        public override IExtents Extents
        {
            get
            {
                Extents b = new Extents(FactoryInternal);

                if (Count == 0)
                {
                    return b;
                }

                _geometries.ForEach(delegate(TGeometry g) { b.ExpandToInclude(g.Extents); });

                return b;
            }
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

            if (other.Count != Count)
            {
                return false;
            }

            unchecked
            {
                for (Int32 i = 0; i < other.Count; i++)
                {
                    if (!other[i].Equals(this[i]))
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
        public override Boolean IsSimple
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the closure of the combinatorial boundary of this Geometry. 
        /// The combinatorial boundary is defined as described in section 3.12.3.2 of [1]. 
        /// Because the result of this function is a closure, and hence topologically closed, 
        /// the resulting boundary can be represented using representational geometry primitives.
        /// </summary>
        /// <returns>Closure of the combinatorial boundary of this Geometry</returns>
        public override IGeometry Boundary
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the shortest distance between any two points in the two geometries
        /// as calculated in the spatial reference system of this Geometry.
        /// </summary>
        /// <param name="geom">Geometry to calculate distance to</param>
        /// <returns>Shortest distance between any two points in the two geometries</returns>
        public override Double Distance(IGeometry geom)
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
        public override IGeometry Buffer(Double d)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        /// <returns>The convex hull</returns>
        public override IGeometry ConvexHull()
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
        public override IGeometry Intersection(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set 
        /// union of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geometry">Geometry to union with</param>
        /// <returns>Unioned geometry</returns>
        public override IGeometry Union(IGeometry geometry)
        {
#warning broken implementation awaiting for replacement with NTS in Beta 2
            if (IsEmpty)
            {
                return geometry;
            }

            if (geometry.IsEmpty)
            {
                return this;
            }

            if (geometry is TGeometry)
            {
                Add((TGeometry)geometry);
                return this;
            }
            else
            {
                GeometryCollection union = new GeometryCollection(2);
                union.Add(this);
                union.Add((Geometry)geometry);
                return union;
            }
        }

        /// <summary>
        /// Returns a geometry that represents the point set 
        /// difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override IGeometry Difference(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set symmetric 
        /// difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override IGeometry SymmetricDifference(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Point> GetVertexes()
        {
            foreach (TGeometry geometry in _geometries)
            {
                foreach (Point point in geometry.GetVertexes())
                {
                    yield return point;
                }
            }
        }

        public override IEnumerable<Point> GetVertexes(ITransformMatrix<DoubleComponent> transform)
        {
            throw new NotImplementedException();
        }

        public void Add(TGeometry geometry)
        {
            _geometries.Add(geometry);
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a deep copy of the GeometryCollection.
        /// </summary>
        /// <returns>A copy of the GeometryCollection instance.</returns>
        public override Geometry Clone()
        {
            return FactoryInternal.CreateGeometryCollection(
                Enumerable.Upcast<IGeometry, TGeometry>(_geometries)) as Geometry;
        }

        #endregion

        #region IEnumerable<TGeometry> Members

        /// <summary>
        /// Gets an enumerator for enumerating the geometries in the GeometryCollection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TGeometry> GetEnumerator()
        {
            foreach (TGeometry g in _geometries)
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

        #region IGeometryCollection Members

        public Int32 Count
        {
            get { return _geometries.Count; }
        }

        public Boolean IsHomogeneous
        {
            get
            {
                OgcGeometryType type = OgcGeometryType.Unknown;

                foreach (TGeometry geometry in _geometries)
                {
                    if (type == OgcGeometryType.Unknown)
                    {
                        type = geometry.GeometryType;
                    }
                    else if(type != geometry.GeometryType)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        #endregion

        #region ICloneable Members

        Object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region IList<IGeometry> Members

        public Int32 IndexOf(IGeometry item)
        {
            throw new NotImplementedException();
        }

        public void Insert(Int32 index, IGeometry item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(Int32 index)
        {
            throw new NotImplementedException();
        }

        IGeometry IList<IGeometry>.this[Int32 index]
        {
            get
            {
                return _geometries[index];
            }
            set
            {
                _geometries[index] = (TGeometry)value;
            }
        }

        #endregion

        #region ICollection<IGeometry> Members

        void ICollection<IGeometry>.Add(IGeometry item)
        {
            Add((TGeometry)item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        void ICollection<IGeometry>.CopyTo(IGeometry[] array, Int32 arrayIndex)
        {
            throw new NotImplementedException();
        }

        public Boolean IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        Boolean ICollection<IGeometry>.Remove(IGeometry item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<IGeometry> Members

        IEnumerator<IGeometry> IEnumerable<IGeometry>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        public override Int32 PointCount
        {
            get
            {
                Int32 pointCount = 0;

                foreach (TGeometry geometry in _geometries)
                {
                    pointCount += geometry.PointCount;    
                }

                return pointCount;
            }
        }

        public override OgcGeometryType GeometryType
        {
            get { return OgcGeometryType.GeometryCollection; }
        }

        protected override bool EqualsInternal(IGeometry other)
        {
            GeometryCollection<TGeometry> g = other as GeometryCollection<TGeometry>;

            Debug.Assert(g != null);

            if (g.Count != Count)
            {
                return false;
            }

            for (Int32 i = 0; i < Count; i++)
            {
                if (!this[i].Equals(g))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
