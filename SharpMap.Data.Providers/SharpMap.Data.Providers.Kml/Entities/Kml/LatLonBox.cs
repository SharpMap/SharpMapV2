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
    [XmlType(TypeName = "LatLonBoxType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("LatLonBox", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LatLonBox : LatLonBoxBase
    {
        private KmlObjectBase[] latLonBoxObjectExtensionGroupField;
        private string[] latLonBoxSimpleExtensionGroupField;
        private double rotationField;

        private bool rotationFieldSpecified;

        public LatLonBox()
        {
            rotationField = 0;
        }

        /// <remarks/>
        public double rotation
        {
            get { return rotationField; }
            set { rotationField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rotationSpecified
        {
            get { return rotationFieldSpecified; }
            set { rotationFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("LatLonBoxSimpleExtensionGroup")]
        public string[] LatLonBoxSimpleExtensionGroup
        {
            get { return latLonBoxSimpleExtensionGroupField; }
            set { latLonBoxSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LatLonBoxObjectExtensionGroup")]
        public KmlObjectBase[] LatLonBoxObjectExtensionGroup
        {
            get { return latLonBoxObjectExtensionGroupField; }
            set { latLonBoxObjectExtensionGroupField = value; }
        }
    }
}