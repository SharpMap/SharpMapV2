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
using GeoAPI.CoordinateSystems;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// The GeographicTransform class is implemented on geographic transformation objects and
    /// implements datum transformations between geographic coordinate systems.
    /// </summary>
    public class GeographicTransform<TCoordinate> : Info, IGeographicTransform<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>,
            IConvertible
    {
        internal GeographicTransform(
            string name, string authority, long code, string alias, string remarks, string abbreviation,
            IGeographicCoordinateSystem<TCoordinate> source, IGeographicCoordinateSystem<TCoordinate> targetGCS)
            : base(name, authority, code, alias, abbreviation, remarks)
        {
            _source = source;
            _target = targetGCS;
        }

        #region IGeographicTransform Members

        private IGeographicCoordinateSystem<TCoordinate> _source;
        private IGeographicCoordinateSystem<TCoordinate> _target;

        /// <summary>
        /// Gets or sets the source geographic coordinate system for the transformation.
        /// </summary>
        public IGeographicCoordinateSystem<TCoordinate> Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Gets or sets the target geographic coordinate system for the transformation.
        /// </summary>
        public IGeographicCoordinateSystem<TCoordinate> Target
        {
            get { return _target; }
            set { _target = value; }
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
        /// Returns the Well-known text for this object
        /// as defined in the simple features specification.
        /// </summary>
        public override string Wkt
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an XML representation of this object [NOT IMPLEMENTED].
        /// </summary>
        public override string Xml
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance.
        /// Only parameters used for coordinate system are used for comparison.
        /// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal</returns>
        public override bool EqualParams(object obj)
        {
            GeographicTransform<TCoordinate> other = obj as GeographicTransform<TCoordinate>;

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return other.Source.EqualParams(Source) && other.Target.EqualParams(Target);
        }

        #endregion
    }
}