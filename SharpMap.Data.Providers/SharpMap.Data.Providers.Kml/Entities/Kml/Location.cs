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
    [XmlType(TypeName = "LocationType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Location", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Location : KmlObjectBase
    {
        private double altitudeField;

        private bool altitudeFieldSpecified;
        private double latitudeField;

        private bool latitudeFieldSpecified;

        private KmlObjectBase[] locationObjectExtensionGroupField;
        private string[] locationSimpleExtensionGroupField;
        private double longitudeField;

        private bool longitudeFieldSpecified;

        public Location()
        {
            longitudeField = 0;
            latitudeField = 0;
            altitudeField = 0;
        }

        /// <remarks/>
        public double longitude
        {
            get { return longitudeField; }
            set { longitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool longitudeSpecified
        {
            get { return longitudeFieldSpecified; }
            set { longitudeFieldSpecified = value; }
        }

        /// <remarks/>
        public double latitude
        {
            get { return latitudeField; }
            set { latitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool latitudeSpecified
        {
            get { return latitudeFieldSpecified; }
            set { latitudeFieldSpecified = value; }
        }

        /// <remarks/>
        public double altitude
        {
            get { return altitudeField; }
            set { altitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool altitudeSpecified
        {
            get { return altitudeFieldSpecified; }
            set { altitudeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("LocationSimpleExtensionGroup")]
        public string[] LocationSimpleExtensionGroup
        {
            get { return locationSimpleExtensionGroupField; }
            set { locationSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LocationObjectExtensionGroup")]
        public KmlObjectBase[] LocationObjectExtensionGroup
        {
            get { return locationObjectExtensionGroupField; }
            set { locationObjectExtensionGroupField = value; }
        }
    }
}