// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of Proj.Net.
// Proj.Net is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Proj.Net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Proj.Net; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// The GeographicTransform class is implemented on geographic transformation objects and
    /// implements datum transformations between geographic coordinate systems.
    /// </summary>
    public class GeographicTransform<TCoordinate> : Info, IGeographicTransform<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>,
            IComputable<Double, TCoordinate>,
            IConvertible
    {
        private readonly IGeographicCoordinateSystem<TCoordinate> _source;
        private readonly IGeographicCoordinateSystem<TCoordinate> _target;

        internal GeographicTransform(
            String name, String authority, long code, String alias, String remarks, String abbreviation,
            IGeographicCoordinateSystem<TCoordinate> source, IGeographicCoordinateSystem<TCoordinate> target)
            : base(name, authority, code, alias, abbreviation, remarks)
        {
            _source = source;
            _target = target;
        }

        #region IGeographicTransform Members

        /// <summary>
        /// Gets or sets the source geographic coordinate system for the transformation.
        /// </summary>
        public IGeographicCoordinateSystem<TCoordinate> Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets or sets the target geographic coordinate system for the transformation.
        /// </summary>
        public IGeographicCoordinateSystem<TCoordinate> Target
        {
            get { return _target; }
        }

        /// <summary>
        /// Returns an accessor interface to the parameters for this geographic transformation.
        /// </summary>
        public IParameterInfo ParameterInfo
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Transforms an array of points from the source geographic coordinate
        /// system to the target geographic coordinate system.
        /// </summary>
        /// <param name="points">On input points in the source geographic coordinate system</param>
        /// <returns>Output points in the target geographic coordinate system</returns>
        public IEnumerable<TCoordinate> Forward(IEnumerable<TCoordinate> points)
        {
            throw new NotImplementedException();
            /*
			List<Point> trans = new List<Point>(points.Count);
			foreach (Point p in points)
			{

			}
			return trans;
			*/
        }

        /// <summary>
        /// Transforms an array of points from the target geographic coordinate
        /// system to the source geographic coordinate system.
        /// </summary>
        /// <param name="points">Input points in the target geographic coordinate system,</param>
        /// <returns>Output points in the source geographic coordinate system</returns>
        public IEnumerable<TCoordinate> Inverse(IEnumerable<TCoordinate> points)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Well-Known Text for this object
        /// as defined in the simple features specification.
        /// </summary>
        public override String Wkt
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an XML representation of this object [NOT IMPLEMENTED].
        /// </summary>
        public override String Xml
        {
            get { throw new NotImplementedException(); }
        }

        public override Boolean EqualParams(IInfo other)
        {
            GeographicTransform<TCoordinate> g = other as GeographicTransform<TCoordinate>;

            if (ReferenceEquals(g, null))
            {
                return false;
            }

            return g.Source.EqualParams(Source)
                   && g.Target.EqualParams(Target);
        }

        #endregion
    }
}