// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Geometries
{
    internal static class BoundingBoxOperations
    {
        internal static Geometry Difference(BoundingBox lhs, BoundingBox rhs)
        {
            BoundingBox union = new BoundingBox(lhs, rhs);

            List<BoundingBox> components = new List<BoundingBox>();
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

            BoundingBox diff = BoundingBox.Join(components.ToArray());

            if (diff.Contains(rhs))
            {
                return lhs.ToGeometry();
            }
            else
            {
                return diff.ToGeometry();
            }
        }

        internal static Geometry Intersection(BoundingBox lhs, BoundingBox rhs)
        {
            return BoundingBox.Intersection(lhs, rhs).ToGeometry();
        }
    }
}
