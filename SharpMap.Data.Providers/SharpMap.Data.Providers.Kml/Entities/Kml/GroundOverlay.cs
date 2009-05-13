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
    [XmlType(TypeName = "GroundOverlayType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("GroundOverlay", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class GroundOverlay : OverlayBase
    {
        private double altitudeField;

        private bool altitudeFieldSpecified;
        private KmlObjectBase[] groundOverlayObjectExtensionGroupField;
        private string[] groundOverlaySimpleExtensionGroupField;

        private AltitudeMode item4Field;

        private LatLonBox latLonBoxField;

        public GroundOverlay()
        {
            altitudeField = 0;
            item4Field = AltitudeMode.ClampToGround;
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
        [XmlElement("altitudeMode")]
        public AltitudeMode Item4
        {
            get { return item4Field; }
            set { item4Field = value; }
        }

        /// <remarks/>
        public LatLonBox LatLonBox
        {
            get { return latLonBoxField; }
            set { latLonBoxField = value; }
        }

        /// <remarks/>
        [XmlElement("GroundOverlaySimpleExtensionGroup")]
        public string[] GroundOverlaySimpleExtensionGroup
        {
            get { return groundOverlaySimpleExtensionGroupField; }
            set { groundOverlaySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("GroundOverlayObjectExtensionGroup")]
        public KmlObjectBase[] GroundOverlayObjectExtensionGroup
        {
            get { return groundOverlayObjectExtensionGroupField; }
            set { groundOverlayObjectExtensionGroupField = value; }
        }
    }
}