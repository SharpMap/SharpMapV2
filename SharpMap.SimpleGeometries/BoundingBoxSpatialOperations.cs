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
using GeoAPI.Geometries;

namespace SharpMap.SimpleGeometries
{
    internal class BoundingBoxSpatialOperations
    {
        private readonly GeometryFactory _factory;

        internal BoundingBoxSpatialOperations(GeometryFactory factory)
        {
            _factory = factory;
        }

        internal Geometry Difference(Extents lhs, Extents rhs)
        {
            Extents union = new Extents(_factory, lhs, rhs);

            List<Extents> components = new List<Extents>();
            components.AddRange(union.Split(rhs.LowerLeft));
            components.AddRange(union.Split(rhs.LowerRight));
            components.AddRange(union.Split(rhs.UpperLeft));
            components.AddRange(union.Split(rhs.UpperRight));

            for (Int32 i = components.Count - 1; i >= 0; i--)
            {
                if (components[i].IsEmpty || rhs.Overlaps(components[i]))
                {
                    components.RemoveAt(i);
                }
            }

            Extents diff = Extents.Join(_factory, components.ToArray());

            if (diff.Contains(rhs))
            {
                return lhs.ToGeometry();
            }
            else
            {
                return diff.ToGeometry();
            }
        }

        internal Geometry Union(Extents lhs, Extents rhs)
        {
            Extents union = new Extents(_factory, lhs, rhs);
            return union.ToGeometry();
        }

        internal Geometry Union(Geometry lhs, Geometry rhs)
        {
            if (lhs == null) throw new ArgumentNullException("lhs");
            if (rhs == null) throw new ArgumentNullException("rhs");

            if (lhs.IsEmpty)
            {
                return rhs;
            }

            if (rhs.IsEmpty)
            {
                return lhs;
            }

            if (lhs.Contains(rhs))
            {
                return lhs;
            }

            if (rhs.Contains(lhs))
            {
                return rhs;
            }

            return new Extents(_factory, lhs.Extents, rhs.Extents).ToGeometry();
        }

        internal IGeometry Union(IGeometry lhs, IGeometry rhs)
        {
            return Union(
                ((Geometry)lhs).ExtentsInternal,
                ((Geometry)rhs).ExtentsInternal);
        }

        internal Geometry Intersection(Extents lhs, Extents rhs)
        {
            return Extents.Intersection(_factory, lhs, rhs).ToGeometry();
        }

        internal Geometry Intersection(Geometry lhs, Geometry rhs)
        {
            return Intersection(lhs.ExtentsInternal, rhs.ExtentsInternal);
        }

        internal IGeometry Intersection(IGeometry lhs, IGeometry rhs)
        {
            return Intersection(
                ((Geometry)lhs).ExtentsInternal, 
                ((Geometry)rhs).ExtentsInternal);
        }
    }
}
