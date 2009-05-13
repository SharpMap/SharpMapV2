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
    [XmlType(TypeName = "LinearRingType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("LinearRing", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LinearRing : GeometryBase
    {
        private string coordinatesField;
        private bool extrudeField;

        private bool extrudeFieldSpecified;

        private AltitudeMode itemField;

        private KmlObjectBase[] linearRingObjectExtensionGroupField;
        private string[] linearRingSimpleExtensionGroupField;
        private bool tessellateField;

        private bool tessellateFieldSpecified;

        public LinearRing()
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
        public string coordinates
        {
            get { return coordinatesField; }
            set { coordinatesField = value; }
        }

        /// <remarks/>
        [XmlElement("LinearRingSimpleExtensionGroup")]
        public string[] LinearRingSimpleExtensionGroup
        {
            get { return linearRingSimpleExtensionGroupField; }
            set { linearRingSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LinearRingObjectExtensionGroup")]
        public KmlObjectBase[] LinearRingObjectExtensionGroup
        {
            get { return linearRingObjectExtensionGroupField; }
            set { linearRingObjectExtensionGroupField = value; }
        }
    }
}