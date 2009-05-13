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
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "PolygonType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Polygon", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Polygon : GeometryBase
    {
        private bool extrudeField;

        private bool extrudeFieldSpecified;
        private Boundary[] innerBoundaryIsField;

        private AltitudeMode itemField;

        private Boundary outerBoundaryIsField;

        private KmlObjectBase[] polygonObjectExtensionGroupField;
        private string[] polygonSimpleExtensionGroupField;
        private bool tessellateField;

        private bool tessellateFieldSpecified;

        public Polygon()
        {
            extrudeField = false;
            tessellateField = false;
            itemField = AltitudeMode.ClampToGround;
        }

        /// <remarks/>
        public bool extrude
        {
            get { return extrudeField; }
            set { extrudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool extrudeSpecified
        {
            get { return extrudeFieldSpecified; }
            set { extrudeFieldSpecified = value; }
        }

        /// <remarks/>
        public bool tessellate
        {
            get { return tessellateField; }
            set { tessellateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool tessellateSpecified
        {
            get { return tessellateFieldSpecified; }
            set { tessellateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public AltitudeMode Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public Boundary outerBoundaryIs
        {
            get { return outerBoundaryIsField; }
            set { outerBoundaryIsField = value; }
        }

        /// <remarks/>
        [XmlElement("innerBoundaryIs")]
        public Boundary[] innerBoundaryIs
        {
            get { return innerBoundaryIsField; }
            set { innerBoundaryIsField = value; }
        }

        /// <remarks/>
        [XmlElement("PolygonSimpleExtensionGroup")]
        public string[] PolygonSimpleExtensionGroup
        {
            get { return polygonSimpleExtensionGroupField; }
            set { polygonSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PolygonObjectExtensionGroup")]
        public KmlObjectBase[] PolygonObjectExtensionGroup
        {
            get { return polygonObjectExtensionGroupField; }
            set { polygonObjectExtensionGroupField = value; }
        }
    }
}