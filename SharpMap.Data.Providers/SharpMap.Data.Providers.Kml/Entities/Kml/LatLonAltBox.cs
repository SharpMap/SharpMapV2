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
    [XmlType(TypeName = "LatLonAltBoxType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("LatLonAltBox", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LatLonAltBox : LatLonBoxBase
    {
        private AltitudeMode itemField;

        private KmlObjectBase[] latLonAltBoxObjectExtensionGroupField;
        private string[] latLonAltBoxSimpleExtensionGroupField;
        private double maxAltitudeField;

        private bool maxAltitudeFieldSpecified;
        private double minAltitudeField;

        private bool minAltitudeFieldSpecified;

        public LatLonAltBox()
        {
            minAltitudeField = 0;
            maxAltitudeField = 0;
            itemField = AltitudeMode.ClampToGround;
        }

        /// <remarks/>
        public double minAltitude
        {
            get { return minAltitudeField; }
            set { minAltitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool minAltitudeSpecified
        {
            get { return minAltitudeFieldSpecified; }
            set { minAltitudeFieldSpecified = value; }
        }

        /// <remarks/>
        public double maxAltitude
        {
            get { return maxAltitudeField; }
            set { maxAltitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxAltitudeSpecified
        {
            get { return maxAltitudeFieldSpecified; }
            set { maxAltitudeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public AltitudeMode Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("LatLonAltBoxSimpleExtensionGroup")]
        public string[] LatLonAltBoxSimpleExtensionGroup
        {
            get { return latLonAltBoxSimpleExtensionGroupField; }
            set { latLonAltBoxSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LatLonAltBoxObjectExtensionGroup")]
        public KmlObjectBase[] LatLonAltBoxObjectExtensionGroup
        {
            get { return latLonAltBoxObjectExtensionGroupField; }
            set { latLonAltBoxObjectExtensionGroupField = value; }
        }
    }
}