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
            Point lowerLeft = new Point(Math.Min(lhs.Left, rhs.Left), Math.Min(lhs.Bottom, rhs.Bottom));
            Point lowerRight = new Point(Math.Max(lhs.Right, rhs.Right), Math.Min(lhs.Bottom, rhs.Bottom));
            Point upperLeft = new Point(Math.Min(lhs.Left, rhs.Left), Math.Max(lhs.Top, rhs.Top));
            Point upperRight = new Point(Math.Max(lhs.Right, rhs.Right), Math.Max(lhs.Top, rhs.Top));

            List<BoundingBox> components = new List<BoundingBox>();
            components.AddRange(lhs.Split(lowerLeft));
            components.AddRange(lhs.Split(lowerRight));
            components.AddRange(lhs.Split(upperLeft));
            components.AddRange(lhs.Split(upperRight));

            for (int i = components.Count - 1; i >= 0; i--)
            {
                if (components[i].IsEmpty)
                {
                    components.RemoveAt(i);
                }
            }

            BoundingBox diff = BoundingBox.Join(components.ToArray());
            return diff.ToGeometry();
        }
    }
}
