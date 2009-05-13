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
    [XmlType(TypeName = "CameraType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Camera", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Camera : ViewBase
    {
        private double altitudeField;

        private bool altitudeFieldSpecified;
        private KmlObjectBase[] cameraObjectExtensionGroupField;
        private string[] cameraSimpleExtensionGroupField;

        private double headingField;

        private bool headingFieldSpecified;
        private AltitudeMode itemField;
        private double latitudeField;

        private bool latitudeFieldSpecified;
        private double longitudeField;

        private bool longitudeFieldSpecified;

        private double rollField;

        private bool rollFieldSpecified;
        private double tiltField;

        private bool tiltFieldSpecified;

        public Camera()
        {
            longitudeField = 0;
            latitudeField = 0;
            altitudeField = 0;
            headingField = 0;
            tiltField = 0;
            rollField = 0;
            itemField = AltitudeMode.ClampToGround;
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
        public double heading
        {
            get { return headingField; }
            set { headingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool headingSpecified
        {
            get { return headingFieldSpecified; }
            set { headingFieldSpecified = value; }
        }

        /// <remarks/>
        public double tilt
        {
            get { return tiltField; }
            set { tiltField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool tiltSpecified
        {
            get { return tiltFieldSpecified; }
            set { tiltFieldSpecified = value; }
        }

        /// <remarks/>
        public double roll
        {
            get { return rollField; }
            set { rollField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rollSpecified
        {
            get { return rollFieldSpecified; }
            set { rollFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public AltitudeMode Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("CameraSimpleExtensionGroup")]
        public string[] CameraSimpleExtensionGroup
        {
            get { return cameraSimpleExtensionGroupField; }
            set { cameraSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("CameraObjectExtensionGroup")]
        public KmlObjectBase[] CameraObjectExtensionGroup
        {
            get { return cameraObjectExtensionGroupField; }
            set { cameraObjectExtensionGroupField = value; }
        }
    }
}