using System;
using System.Collections.Generic;
using System.Text;
/*
 *  The attached / following is part of SharpMap
 *  SharpMap is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */


namespace SharpMap.Expressions
{
    [Serializable]
    public enum SpatialAnalysisOperation
    {
        None,
        /// <summary>
        /// Returns the shortest distance between any two Points in the two geometric objects as calculated in the spatial 
        /// reference system of this geometric object. Because the
        /// geometries are closed, it is possible to find a point on each geometric object involved, such that the distance
        /// between these 2 points is the returned distance between their geometric objects
        /// </summary>
        Distance,
        /// <summary>
        /// Returns a geometric object that represents all Points whose distance
        /// from this geometric object is less than or equal to distance. Calculations are in the spatial reference system of
        /// this geometric object. Because of the limitations of linear interpolation, there will often be some relatively
        /// small error in this distance, but it should be near the resolution of the coordinates used.
        /// </summary>
        Buffer,
        /// <summary>
        /// Returns a geometric object that represents the convex hull of this geometric
        /// object. Convex hulls, being dependent on straight lines, can be accurately represented in linear interpolations
        /// for any geometry restricted to linear interpolations.
        /// </summary>
        ConvexHull,
        /// <summary>
        /// Returns a geometric object that represents the
        /// Point set intersection of this geometric object with anotherGeometry
        /// </summary>
        Intersection,
        /// <summary>
        /// Returns a geometric object that represents the Point set
        /// union of this geometric object with anotherGeometry.
        /// </summary>
        Union,
        /// <summary>
        /// Returns a geometric object that represents the Point
        /// set difference of this geometric object with anotherGeometry.
        /// </summary>
        Difference,
        /// <summary>
        /// Returns a geometric object that represents the
        /// Point set symmetric difference of this geometric object with anotherGeometry
        /// </summary>
        SymDifference
    }
}
