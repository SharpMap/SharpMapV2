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
using GeoAPI.Coordinates;
using GeoAPI.Geometries;

namespace SharpMap.SimpleGeometries
{
    /// <summary>
    /// A LinearRing is a LineString that is both closed and simple.
    /// </summary>
    public class LinearRing : LineString, ILinearRing
    {
        /// <summary>
        /// Initializes an instance of a LinearRing from a set of vertices
        /// </summary>
        /// <param name="vertices"></param>
        public LinearRing(IEnumerable<IPoint> vertices)
            : base(vertices) { }

        /// <summary>
        /// Initializes an instance of a LinearRing
        /// </summary>
        public LinearRing() { }

        /// <summary>
        /// Creates a deep copy of the LinearRing.
        /// </summary>
        /// <returns>A copy of the LinearRing instance.</returns>
        public override Geometry Clone()
        {
            LinearRing ring = new LinearRing(Vertices);
            return ring;
        }

        /// <summary>
        /// Tests whether a ring is oriented counter-clockwise.
        /// </summary>
        /// <returns>Returns true if ring is oriented counter-clockwise.</returns>
        public Boolean IsCcw()
        {
            IPoint hip, p, prev, next;
            Int32 hii, i;
            Int32 nPts = Vertices.Count;

            // check that this is a valid ring - if not, simply return a dummy value
            if (nPts < 4)
            {
                return false;
            }

            // algorithm to check if a Ring is stored in CCW order
            // find highest point
            hip = Vertices[0];
            hii = 0;

            for (i = 1; i < nPts; i++)
            {
                p = Vertices[i];

                if (p[Ordinates.Y] > hip[Ordinates.Y])
                {
                    hip = p;
                    hii = i;
                }
            }

            // find points on either side of highest
            Int32 iPrev = hii - 1;

            if (iPrev < 0)
            {
                iPrev = nPts - 2;
            }

            Int32 iNext = hii + 1;

            if (iNext >= nPts)
            {
                iNext = 1;
            }

            prev = Vertices[iPrev];
            next = Vertices[iNext];

            // translate so that hip is at the origin.
            // This will not affect the area calculation, and will avoid
            // finite-accuracy errors (i.e very small vectors with very large coordinates)
            // This also simplifies the discriminant calculation.
            Double prev2x = prev[Ordinates.X] - hip[Ordinates.X];
            Double prev2y = prev[Ordinates.Y] - hip[Ordinates.Y];
            Double next2x = next[Ordinates.X] - hip[Ordinates.X];
            Double next2y = next[Ordinates.Y] - hip[Ordinates.Y];

            // compute determinant of vectors hip->next and hip->prev
            // (e.g. area of parallelogram they enclose)
            Double disc = next2x * prev2y - next2y * prev2x;

            // If disc is exactly 0, lines are collinear.  There are two possible cases:
            //	(1) the lines lie along the x axis in opposite directions
            //	(2) the line lie on top of one another
            //  (2) should never happen, so we're going to ignore it!
            //	(Might want to assert this)
            //  (1) is handled by checking if next is left of prev ==> CCW
            if (disc == 0.0)
            {
                // poly is CCW if prev x is right of next x
                return (prev[Ordinates.X] > next[Ordinates.X]);
            }
            else
            {
                // if area is positive, points are ordered CCW
                return (disc > 0.0);
            }
        }

        /// <summary>
        /// Returns the area of the LinearRing
        /// </summary>
        public Double Area
        {
            get
            {
                if (Vertices.Count < 3)
                {
                    return 0;
                }

                Double sum = 0;
                Double ax = Vertices[0][Ordinates.X];
                Double ay = Vertices[0][Ordinates.Y];

                for (Int32 i = 1; i < Vertices.Count - 1; i++)
                {
                    Double bx = Vertices[i][Ordinates.X];
                    Double by = Vertices[i][Ordinates.Y];
                    Double cx = Vertices[i + 1][Ordinates.X];
                    Double cy = Vertices[i + 1][Ordinates.Y];

                    sum += ax * by - ay * bx +
                           ay * cx - ax * cy +
                           bx * cy - cx * by;
                }

                return Math.Abs(-sum / 2);
            }
        }

        /// <summary>
        /// Returns true of the Point 'p' is within the instance of this ring
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Boolean IsPointWithin(Point p)
        {
            Boolean c = false;
            for (Int32 i = 0; i < Vertices.Count; i++)
            {
                for (Int32 j = i + 1; j < Vertices.Count - 1; j++)
                {
                    // this is horrible code to read. If the unit tests were working,
                    // I could fix it without worrying I'd break something!
                    if ((((Vertices[i][Ordinates.Y] <= p.Y) && (p.Y < Vertices[j][Ordinates.Y])) ||
                         ((Vertices[j][Ordinates.Y] <= p.Y) && (p.Y < Vertices[i][Ordinates.Y]))) &&
                        (p.X < (Vertices[j][Ordinates.X] - Vertices[i][Ordinates.X]) 
                            * (p.Y - Vertices[i][Ordinates.Y]) / (Vertices[j][Ordinates.Y] 
                            - Vertices[i][Ordinates.Y]) + Vertices[i][Ordinates.X]))
                    {
                        c = !c;
                    }
                }
            }
            return c;
        }

        #region ILinearRing Members

        Boolean ILinearRing.IsCcw
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}