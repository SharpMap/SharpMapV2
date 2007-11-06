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
using System.Globalization;
using System.Text;
using GeoAPI.CoordinateSystems;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// A 2D cartographic coordinate system.
    /// </summary>
    public class ProjectedCoordinateSystem<TCoordinate> : HorizontalCoordinateSystem<TCoordinate>,
                                                          IProjectedCoordinateSystem<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>,
            IConvertible
    {
        /// <summary>
        /// Initializes a new instance of a projected coordinate system
        /// </summary>
        /// <param name="datum">Horizontal datum</param>
        /// <param name="geographicCoordinateSystem">Geographic coordinate system</param>
        /// <param name="linearUnit">Linear unit</param>
        /// <param name="projection">Projection</param>
        /// <param name="axisInfo">Axis info</param>
        /// <param name="name">Name</param>
        /// <param name="authority">Authority name</param>
        /// <param name="code">Authority-specific identification code.</param>
        /// <param name="alias">Alias</param>
        /// <param name="abbreviation">Abbreviation</param>
        /// <param name="remarks">Provider-supplied remarks</param>
        internal ProjectedCoordinateSystem(IHorizontalDatum datum,
                                           IGeographicCoordinateSystem<TCoordinate> geographicCoordinateSystem,
                                           ILinearUnit linearUnit, IProjection projection, IEnumerable<AxisInfo> axisInfo,
                                           String name, String authority, long code, String alias,
                                           String remarks, String abbreviation)
            : base(datum, axisInfo, name, authority, code, alias, abbreviation, remarks)
        {
            _geographicCoordinateSystem = geographicCoordinateSystem;
            _linearUnit = linearUnit;
            _projection = projection;
        }

        #region Predefined projected coordinate systems

        /*
		/// <summary>
		/// Universal Transverse Mercator - WGS84
		/// </summary>
		/// <param name="Zone">UTM zone</param>
		/// <param name="ZoneIsNorth">true of Northern hemisphere, false if southern</param>
		/// <returns>UTM/WGS84 coordsys</returns>
		public static ProjectedCoordinateSystem WGS84_UTM(Int32 Zone, Boolean ZoneIsNorth)
		{
			ParameterInfo pInfo = new ParameterInfo();
			pInfo.Add("latitude_of_origin", 0);
			pInfo.Add("central_meridian", Zone * 6 - 183);
			pInfo.Add("scale_factor", 0.9996);
			pInfo.Add("false_easting", 500000);
			pInfo.Add("false_northing", ZoneIsNorth ? 0 : 10000000);
			Projection proj = new Projection(String.Empty,String.Empty,pInfo,AngularUnit.Degrees,
				SharpMap.SpatialReference.LinearUnit.Metre,Ellipsoid.WGS84,
				"Transverse_Mercator", "EPSG", 32600 + Zone + (ZoneIsNorth ? 0 : 100), String.Empty, String.Empty, String.Empty);

			return new ProjectedCoordinateSystem("Large and medium scale topographic mapping and engineering survey.",
				SharpMap.SpatialReference.GeographicCoordinateSystem.WGS84,
				SharpMap.SpatialReference.LinearUnit.Metre, proj, pInfo,
				"WGS 84 / UTM zone " + Zone.ToString() + (ZoneIsNorth ? "N" : "S"), "EPSG", 32600 + Zone + (ZoneIsNorth ? 0 : 100),
				String.Empty,String.Empty,String.Empty);
			
		}*/

        #endregion

        #region IProjectedCoordinateSystem Members

        private IGeographicCoordinateSystem<TCoordinate> _geographicCoordinateSystem;

        /// <summary>
        /// Gets or sets the GeographicCoordinateSystem.
        /// </summary>
        public IGeographicCoordinateSystem<TCoordinate> GeographicCoordinateSystem
        {
            get { return _geographicCoordinateSystem; }
            set { _geographicCoordinateSystem = value; }
        }

        private ILinearUnit _linearUnit;

        /// <summary>
        /// Gets or sets the <see cref="LinearUnit">LinearUnits</see>. 
        /// The linear unit must be the same as the 
        /// <see cref="CoordinateSystem{TCoordinate}"/> units.
        /// </summary>
        public ILinearUnit LinearUnit
        {
            get { return _linearUnit; }
            set { _linearUnit = value; }
        }

        /// <summary>
        /// Gets units for dimension within coordinate system. Each dimension in 
        /// the coordinate system has corresponding units.
        /// </summary>
        /// <param name="dimension">Dimension</param>
        /// <returns>Unit</returns>
        public override IUnit GetUnits(Int32 dimension)
        {
            return _linearUnit;
        }

        private IProjection _projection;

        /// <summary>
        /// Gets or sets the projection
        /// </summary>
        public IProjection Projection
        {
            get { return _projection; }
            set { _projection = value; }
        }

        /// <summary>
        /// Returns the Well-Known Text for this object
        /// as defined in the simple features specification.
        /// </summary>
        public override String Wkt
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("PROJCS[\"{0}\", {1}, {2}", Name, GeographicCoordinateSystem.Wkt, Projection.Wkt);

                foreach (ProjectionParameter parameter in _projection)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, ", {0}", parameter.Wkt);
                }

                sb.AppendFormat(", {0}", LinearUnit.Wkt);

                //Skip axis info if they contain default values
                if (AxisInfo.Count != 2 ||
                    AxisInfo[0].Name != "X" || AxisInfo[0].Orientation != AxisOrientation.East ||
                    AxisInfo[1].Name != "Y" || AxisInfo[1].Orientation != AxisOrientation.North)
                {
                    foreach (AxisInfo info in AxisInfo)
                    {
                        sb.AppendFormat(", {0}", info.Wkt);
                    }
                }

                if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
                {
                    sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
                }

                sb.Append("]");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets an XML representation of this object.
        /// </summary>
        public override String Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat,
                                "<CS_CoordinateSystem Dimension=\"{0}\"><CS_ProjectedCoordinateSystem>{1}",
                                Dimension, InfoXml);

                foreach (AxisInfo ai in AxisInfo)
                {
                    sb.Append(ai.Xml);
                }

                sb.AppendFormat("{0}{1}{2}</CS_ProjectedCoordinateSystem></CS_CoordinateSystem>",
                                GeographicCoordinateSystem.Xml, LinearUnit.Xml, Projection.Xml);

                return sb.ToString();
            }
        }

        public override Boolean EqualParams(IInfo other)
        {
            ProjectedCoordinateSystem<TCoordinate> p = other as ProjectedCoordinateSystem<TCoordinate>;

            if (ReferenceEquals(p, null))
            {
                return false;
            }

            if (p.Dimension != Dimension)
            {
                return false;
            }

            for (Int32 i = 0; i < p.Dimension; i++)
            {
                if (p.GetAxis(i).Orientation != GetAxis(i).Orientation)
                {
                    return false;
                }
                if (!p.GetUnits(i).EqualParams(GetUnits(i)))
                {
                    return false;
                }
            }

            return p.GeographicCoordinateSystem.EqualParams(GeographicCoordinateSystem) &&
                   p.HorizontalDatum.EqualParams(HorizontalDatum) &&
                   p.LinearUnit.EqualParams(LinearUnit) &&
                   p.Projection.EqualParams(Projection);
        }

        #endregion
    }
}