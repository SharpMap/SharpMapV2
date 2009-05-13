// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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
using System.ComponentModel;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "vec2Type", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("hotSpot", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Vector2
    {
        public Vector2()
        {
            X = 1;
            Y = 1;
            XUnit = Unit.Fraction;
            YUnit = Unit.Fraction;
        }

        /// <remarks/>
        [XmlAttribute("x"), DefaultValue(1)]
        public double X { get; set; }

        /// <remarks/>
        [XmlAttribute("y"), DefaultValue(1)]
        public double Y { get; set; }

        /// <remarks/>
        [XmlAttribute("xunits"), DefaultValue(Unit.Fraction)]
        public Unit XUnit { get; set; }

        /// <remarks/>
        [XmlAttribute("yunits"), DefaultValue(Unit.Fraction)]
        public Unit YUnit { get; set; }
    }
}