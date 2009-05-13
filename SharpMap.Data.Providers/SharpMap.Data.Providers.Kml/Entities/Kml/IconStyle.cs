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
    [XmlType(TypeName = "IconStyleType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("IconStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class IconStyle : ColorStyleBase
    {
        private double headingField;

        private bool headingFieldSpecified;

        private Vector2 hotSpotField;
        private BasicLink iconField;

        private KmlObjectBase[] iconStyleObjectExtensionGroupField;
        private string[] iconStyleSimpleExtensionGroupField;
        private double scaleField;

        private bool scaleFieldSpecified;

        public IconStyle()
        {
            scaleField = 1;
            headingField = 0;
        }

        /// <remarks/>
        public double scale
        {
            get { return scaleField; }
            set { scaleField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool scaleSpecified
        {
            get { return scaleFieldSpecified; }
            set { scaleFieldSpecified = value; }
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
        public BasicLink Icon
        {
            get { return iconField; }
            set { iconField = value; }
        }

        /// <remarks/>
        public Vector2 hotSpot
        {
            get { return hotSpotField; }
            set { hotSpotField = value; }
        }

        /// <remarks/>
        [XmlElement("IconStyleSimpleExtensionGroup")]
        public string[] IconStyleSimpleExtensionGroup
        {
            get { return iconStyleSimpleExtensionGroupField; }
            set { iconStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("IconStyleObjectExtensionGroup")]
        public KmlObjectBase[] IconStyleObjectExtensionGroup
        {
            get { return iconStyleObjectExtensionGroupField; }
            set { iconStyleObjectExtensionGroupField = value; }
        }
    }
}