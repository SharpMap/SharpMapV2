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

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Geometries;

namespace SharpMap.CoordinateSystems.Projections
{
    /// <summary>
    /// Projections inherit from this abstract class to get access to useful mathematical functions.
    /// </summary>
    internal abstract class MapProjection : MathTransform, IProjection
    {
        protected Boolean _isInverse = false;
        protected Boolean _isSpherical = false;
        protected Double _e;
        protected Double _es;
        protected Double _semiMajor;
        protected Double _semiMinor;

        protected List<ProjectionParameter> _Parameters;
        protected MathTransform _inverse;

        protected MapProjection(List<ProjectionParameter> parameters, Boolean isInverse)
            : this(parameters)
        {
            _isInverse = isInverse;
        }

        protected MapProjection(List<ProjectionParameter> parameters)
        {
            _Parameters = parameters;
            //todo. Should really convert to the correct linear units??
            ProjectionParameter semimajor = GetParameter("semi_major");
            ProjectionParameter semiminor = GetParameter("semi_minor");
            if (semimajor == null)
            {
                throw new ArgumentException("Missing projection parameter 'semi_major'");
            }
            if (semiminor == null)
            {
                throw new ArgumentException("Missing projection parameter 'semi_minor'");
            }
            _semiMajor = semimajor.Value;
            _semiMinor = semiminor.Value;

            _isSpherical = (_semiMajor == _semiMinor);
            _es = 1.0 - (_semiMinor*_semiMinor)/(_semiMajor*_semiMajor);
            _e = Math.Sqrt(_es);
        }

        #region Implementation of IProjection

        public ProjectionParameter GetParameter(Int32 Index)
        {
            return _Parameters[Index];
        }

        /// <summary>
        /// Gets an named parameter of the projection.
        /// </summary>
        /// <remarks>The parameter name is case insensitive</remarks>
        /// <param name="name">Name of parameter</param>
        /// <returns>parameter or null if not found</returns>
        public ProjectionParameter GetParameter(String name)
        {
            return
                _Parameters.Find(
                    delegate(ProjectionParameter par) { return par.Name.Equals(name, StringComparison.OrdinalIgnoreCase); });
        }

        public Int32 NumParameters
        {
            get { return _Parameters.Count; }
        }

        public String ClassName
        {
            get { return ClassName; }
        }

        private String _Abbreviation;

        /// <summary>
        /// Gets or sets the abbreviation of the object.
        /// </summary>
        public String Abbreviation
        {
            get { return _Abbreviation; }
            set { _Abbreviation = value; }
        }

        private String _Alias;

        /// <summary>
        /// Gets or sets the alias of the object.
        /// </summary>
        public String Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }

        private String _Authority;

        /// <summary>
        /// Gets or sets the authority name for this object, e.g., "EPSG",
        /// is this is a standard object with an authority specific
        /// identity code. Returns "CUSTOM" if this is a custom object.
        /// </summary>
        public String Authority
        {
            get { return _Authority; }
            set { _Authority = value; }
        }

        private Int64 _Code;

        /// <summary>
        /// Gets or sets the authority specific identification code of the object
        /// </summary>
        public Int64 AuthorityCode
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private String _Name;

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private String _Remarks;

        /// <summary>
        /// Gets or sets the provider-supplied remarks for the object.
        /// </summary>
        public String Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }


        /// <summary>
        /// Returns the Well-known text for this object
        /// as defined in the simple features specification.
        /// </summary>
        public override String Wkt
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (_isInverse)
                {
                    sb.Append("INVERSE_MT[");
                }
                sb.AppendFormat("PARAM_MT[\"{0}\"", Name);
                for (Int32 i = 0; i < NumParameters; i++)
                {
                    sb.AppendFormat(", {0}", GetParameter(i).WKT);
                }
                //if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
                //	sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
                sb.Append("]");
                if (_isInverse)
                {
                    sb.Append("]");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets an XML representation of this object
        /// </summary>
        public override String Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<CT_MathTransform>");
                if (_isInverse)
                {
                    sb.AppendFormat("<CT_InverseTransform Name=\"{0}\">", ClassName);
                }
                else
                {
                    sb.AppendFormat("<CT_ParameterizedMathTransform Name=\"{0}\">", ClassName);
                }
                for (Int32 i = 0; i < NumParameters; i++)
                {
                    sb.AppendFormat(GetParameter(i).XML);
                }
                if (_isInverse)
                {
                    sb.Append("</CT_InverseTransform>");
                }
                else
                {
                    sb.Append("</CT_ParameterizedMathTransform>");
                }
                sb.Append("</CT_MathTransform>");
                return sb.ToString();
            }
        }

        #endregion

        #region IMathTransform

        public abstract Point MetersToDegrees(Point p);
        public abstract Point DegreesToMeters(Point lonlat);

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public override void Invert()
        {
            _isInverse = !_isInverse;
        }

        /// <summary>
        /// Returns true if this projection is inverted.
        /// Most map projections define forward projection as "from geographic to projection", and backwards
        /// as "from projection to geographic". If this projection is inverted, this will be the other way around.
        /// </summary>
        internal Boolean IsInverse
        {
            get { return _isInverse; }
        }

        public override Point Transform(Point cp)
        {
            if (!_isInverse)
            {
                return DegreesToMeters(cp);
            }
            else
            {
                return MetersToDegrees(cp);
            }
        }

        public override IEnumerable<Point> TransformList(IEnumerable<Point> ord)
        {
            foreach (Point point in ord)
            {
                yield return Transform(point);
            }
        }

        /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance.
        /// Only parameters used for coordinate system are used for comparison.
        /// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal</returns>
        public Boolean EqualParams(object obj)
        {
            if (!(obj is MapProjection))
            {
                return false;
            }
            MapProjection proj = obj as MapProjection;
            if (proj.NumParameters != NumParameters)
            {
                return false;
            }
            for (Int32 i = 0; i < _Parameters.Count; i++)
            {
                ProjectionParameter param =
                    _Parameters.Find(
                        delegate(ProjectionParameter par) { return par.Name.Equals(proj.GetParameter(i).Name, StringComparison.OrdinalIgnoreCase); });
                if (param == null)
                {
                    return false;
                }
                if (param.Value != proj.GetParameter(i).Value)
                {
                    return false;
                }
            }
            if (IsInverse != proj.IsInverse)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Helper mathmatical functions

        // defines some usefull constants that are used in the projection routines
        /// <summary>
        /// PI
        /// </summary>
        protected const Double PI = Math.PI;

        /// <summary>
        /// Half of PI
        /// </summary>
        protected const Double HALF_PI = (PI*0.5);

        /// <summary>
        /// PI * 2
        /// </summary>
        protected const Double TWO_PI = (PI*2.0);

        /// <summary>
        /// EPSLN
        /// </summary>
        protected const Double EPSLN = 1.0e-10;

        /// <summary>
        /// S2R
        /// </summary>
        protected const Double S2R = 4.848136811095359e-6;

        /// <summary>
        /// MAX_VAL
        /// </summary>
        protected const Double MAX_VAL = 4;

        /// <summary>
        /// prjMAXLONG
        /// </summary>
        protected const Double prjMAXLONG = 2147483647;

        /// <summary>
        /// DBLLONG
        /// </summary>
        protected const Double DBLLONG = 4.61168601e18;

        /// <summary>
        /// Returns the cube of a number.
        /// </summary>
        /// <param name="x"> </param>
        protected static Double CUBE(Double x)
        {
            return Math.Pow(x, 3); /* x^3 */
        }

        /// <summary>
        /// Returns the quad of a number.
        /// </summary>
        /// <param name="x"> </param>
        protected static Double QUAD(Double x)
        {
            return Math.Pow(x, 4); /* x^4 */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static Double GMAX(ref Double A, ref Double B)
        {
            return Math.Max(A, B); /* assign maximum of a and b */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static Double GMIN(ref Double A, ref Double B)
        {
            return ((A) < (B) ? (A) : (B)); /* assign minimum of a and b */
        }

        /// <summary>
        /// IMOD
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static Double IMOD(Double A, Double B)
        {
            return (A) - (((A)/(B))*(B)); /* Integer mod function */
        }

        ///<summary>
        ///Function to return the sign of an argument
        ///</summary>
        protected static Double sign(Double x)
        {
            if (x < 0.0)
            {
                return (-1);
            }
            else
            {
                return (1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static Double adjust_lon(Double x)
        {
            Int64 count = 0;
            for (;;)
            {
                if (Math.Abs(x) <= PI)
                {
                    break;
                }
                else if (((Int64) Math.Abs(x/Math.PI)) < 2)
                {
                    x = x - (sign(x)*TWO_PI);
                }
                else if (((Int64) Math.Abs(x/TWO_PI)) < prjMAXLONG)
                {
                    x = x - (((Int64) (x/TWO_PI))*TWO_PI);
                }
                else if (((Int64) Math.Abs(x/(prjMAXLONG*TWO_PI))) < prjMAXLONG)
                {
                    x = x - (((Int64) (x/(prjMAXLONG*TWO_PI)))*(TWO_PI*prjMAXLONG));
                }
                else if (((Int64) Math.Abs(x/(DBLLONG*TWO_PI))) < prjMAXLONG)
                {
                    x = x - (((Int64) (x/(DBLLONG*TWO_PI)))*(TWO_PI*DBLLONG));
                }
                else
                {
                    x = x - (sign(x)*TWO_PI);
                }
                count++;
                if (count > MAX_VAL)
                {
                    break;
                }
            }
            return (x);
        }

        /// <summary>
        /// Function to compute the constant small m which is the radius of
        /// a parallel of latitude, phi, divided by the semimajor axis.
        /// </summary>
        protected static Double msfnz(Double eccent, Double sinphi, Double cosphi)
        {
            Double con;

            con = eccent*sinphi;
            return ((cosphi/(Math.Sqrt(1.0 - con*con))));
        }

        /// <summary>
        /// Function to compute constant small q which is the radius of a 
        /// parallel of latitude, phi, divided by the semimajor axis. 
        /// </summary>
        protected static Double qsfnz(Double eccent, Double sinphi, Double cosphi)
        {
            Double con;

            if (eccent > 1.0e-7)
            {
                con = eccent*sinphi;
                return ((1.0 - eccent*eccent)*(sinphi/(1.0 - con*con) - (.5/eccent)*
                                                                        Math.Log((1.0 - con)/(1.0 + con))));
            }
            else
            {
                return (2.0*sinphi);
            }
        }

        /// <summary>
        /// Function to calculate the sine and cosine in one call.  Some computer
        /// systems have implemented this function, resulting in a faster implementation
        /// than calling each function separately.  It is provided here for those
        /// computer systems which don`t implement this function
        /// </summary>
        protected static void sincos(Double val, out Double sin_val, out Double cos_val)

        {
            sin_val = Math.Sin(val);
            cos_val = Math.Cos(val);
        }

        /// <summary>
        /// Function to compute the constant small t for use in the forward
        /// computations in the Lambert Conformal Conic and the Polar
        /// Stereographic projections.
        /// </summary>
        protected static Double tsfnz(Double eccent,
                                      Double phi,
                                      Double sinphi
            )
        {
            Double con;
            Double com;

            con = eccent*sinphi;
            com = .5*eccent;
            con = Math.Pow(((1.0 - con)/(1.0 + con)), com);
            return (Math.Tan(.5*(HALF_PI - phi))/con);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="eccent"></param>
        /// <param name="qs"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected static Double phi1z(Double eccent, Double qs, out Int64 flag)
        {
            Double eccnts;
            Double dphi;
            Double con;
            Double com;
            Double sinpi;
            Double cospi;
            Double phi;
            flag = 0;
            //Double asinz();
            Int64 i;

            phi = asinz(.5*qs);
            if (eccent < EPSLN)
            {
                return (phi);
            }
            eccnts = eccent*eccent;
            for (i = 1; i <= 25; i++)
            {
                sincos(phi, out sinpi, out cospi);
                con = eccent*sinpi;
                com = 1.0 - con*con;
                dphi = .5*com*com/cospi*(qs/(1.0 - eccnts) - sinpi/com +
                                         .5/eccent*Math.Log((1.0 - con)/(1.0 + con)));
                phi = phi + dphi;
                if (Math.Abs(dphi) <= 1e-7)
                {
                    return (phi);
                }
            }
            //p_error ("Convergence error","phi1z-conv");
            //ASSERT(FALSE);
            throw new ProjectionComputationException("Convergence error.");
        }

        ///<summary>
        ///Function to eliminate roundoff errors in asin
        ///</summary>
        protected static Double asinz(Double con)
        {
            if (Math.Abs(con) > 1.0)
            {
                if (con > 1.0)
                {
                    con = 1.0;
                }
                else
                {
                    con = -1.0;
                }
            }
            return (Math.Asin(con));
        }


        /// <summary>Function to compute the latitude angle, phi2, for the inverse of the
        ///   Lambert Conformal Conic and Polar Stereographic projections.
        ///   </summary>
        protected static Double phi2z(Double eccent, Double ts, out Int64 flag)
            /* Spheroid eccentricity		*/
            /* Constant value t			*/
            /* Error flag number			*/

        {
            Double con;
            Double dphi;
            Double sinpi;
            Int64 i;

            flag = 0;
            Double eccnth = .5*eccent;
            Double chi = HALF_PI - 2*Math.Atan(ts);
            for (i = 0; i <= 15; i++)
            {
                sinpi = Math.Sin(chi);
                con = eccent*sinpi;
                dphi = HALF_PI - 2*Math.Atan(ts*(Math.Pow(((1.0 - con)/(1.0 + con)), eccnth))) - chi;
                chi += dphi;
                if (Math.Abs(dphi) <= .0000000001)
                {
                    return (chi);
                }
            }
            throw new ProjectionComputationException("Convergence error - phi2z-conv");
        }


        ///<summary>
        ///Functions to compute the constants e0, e1, e2, and e3 which are used
        ///in a series for calculating the distance along a meridian.  The
        ///input x represents the eccentricity squared.
        ///</summary>
        protected static Double e0fn(Double x)
        {
            return (1.0 - 0.25*x*(1.0 + x/16.0*(3.0 + 1.25*x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static Double e1fn(Double x)
        {
            return (0.375*x*(1.0 + 0.25*x*(1.0 + 0.46875*x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static Double e2fn(Double x)
        {
            return (0.05859375*x*x*(1.0 + 0.75*x));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static Double e3fn(Double x)
        {
            return (x*x*x*(35.0/3072.0));
        }

        /// <summary>
        /// Function to compute the constant e4 from the input of the eccentricity
        /// of the spheroid, x.  This constant is used in the Polar Stereographic
        /// projection.
        /// </summary>
        protected static Double e4fn(Double x)

        {
            Double con;
            Double com;
            con = 1.0 + x;
            com = 1.0 - x;
            return (Math.Sqrt((Math.Pow(con, con))*(Math.Pow(com, com))));
        }

        /// <summary>
        /// Function computes the value of M which is the distance along a meridian
        /// from the Equator to latitude phi.
        /// </summary>
        protected static Double mlfn(Double e0, Double e1, Double e2, Double e3, Double phi)
        {
            return (e0*phi - e1*Math.Sin(2.0*phi) + e2*Math.Sin(4.0*phi) - e3*Math.Sin(6.0*phi));
        }

        /// <summary>
        /// Function to calculate UTM zone number--NOTE Longitude entered in DEGREES!!!
        /// </summary>
        protected static Int64 calc_utm_zone(Double lon)
        {
            return ((Int64) (((lon + 180.0)/6.0) + 1.0));
        }

        #endregion

        #region Static Methods;

        /// <summary>
        /// Converts a longitude value in degrees to radians.
        /// </summary>
        /// <param name="x">The value in degrees to convert to radians.</param>
        /// <param name="edge">If true, -180 and +180 are valid, otherwise they are considered out of range.</param>
        /// <returns></returns>
        protected static Double LongitudeToRadians(Double x, Boolean edge)
        {
            if (edge ? (x >= -180 && x <= 180) : (x > -180 && x < 180))
            {
                return Degrees2Radians(x);
            }
            throw new ArgumentOutOfRangeException("x", x, " not a valid longitude in degrees.");
        }


        /// <summary>
        /// Converts a latitude value in degrees to radians.
        /// </summary>
        /// <param name="y">The value in degrees to to radians.</param>
        /// <param name="edge">If true, -90 and +90 are valid, otherwise they are considered out of range.</param>
        /// <returns></returns>
        protected static Double LatitudeToRadians(Double y, Boolean edge)
        {
            if (edge ? (y >= -90 && y <= 90) : (y > -90 && y < 90))
            {
                return Degrees2Radians(y);
            }
            throw new ArgumentOutOfRangeException("x", y, " not a valid latitude in degrees.");
        }

        #endregion
    }
}