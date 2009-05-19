// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable]
    public enum CurveInterpolationType
    {
        [XmlEnum(Name = "circularArc2PointWithBulge")] CircularArc2PointWithBulge = 3,
        [XmlEnum(Name = "circularArc3Points")] CircularArc3Points = 2,
        [XmlEnum(Name = "circularArcCenterPointWithRadius")] CircularArcCenterPointWithRadius = 4,
        [XmlEnum(Name = "clothoid")] Clothoid = 6,
        [XmlEnum(Name = "conic")] Conic = 7,
        [XmlEnum(Name = "cubicSpline")] CubicSpline = 9,
        [XmlEnum(Name = "elliptical")] Elliptical = 5,
        [XmlEnum(Name = "geodesic")] Geodesic = 1,
        [XmlEnum(Name = "linear")] Linear = 0,
        [XmlEnum(Name = "polynomialSpline")] PolynomialSpline = 8,
        [XmlEnum(Name = "rationalSpline")] RationalSpline = 10
    }
}