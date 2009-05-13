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
    [XmlType(TypeName = "ModelType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Model", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Model : GeometryBase
    {
        private AltitudeMode itemField;
        private LinkType linkField;

        private Location locationField;
        private KmlObjectBase[] modelObjectExtensionGroupField;
        private string[] modelSimpleExtensionGroupField;

        private Orientation orientationField;

        private ResourceMap resourceMapField;
        private Scale scaleField;

        public Model()
        {
            itemField = AltitudeMode.ClampToGround;
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public AltitudeMode Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public Location Location
        {
            get { return locationField; }
            set { locationField = value; }
        }

        /// <remarks/>
        public Orientation Orientation
        {
            get { return orientationField; }
            set { orientationField = value; }
        }

        /// <remarks/>
        public Scale Scale
        {
            get { return scaleField; }
            set { scaleField = value; }
        }

        /// <remarks/>
        public LinkType Link
        {
            get { return linkField; }
            set { linkField = value; }
        }

        /// <remarks/>
        public ResourceMap ResourceMap
        {
            get { return resourceMapField; }
            set { resourceMapField = value; }
        }

        /// <remarks/>
        [XmlElement("ModelSimpleExtensionGroup")]
        public string[] ModelSimpleExtensionGroup
        {
            get { return modelSimpleExtensionGroupField; }
            set { modelSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ModelObjectExtensionGroup")]
        public KmlObjectBase[] ModelObjectExtensionGroup
        {
            get { return modelObjectExtensionGroupField; }
            set { modelObjectExtensionGroupField = value; }
        }
    }
}